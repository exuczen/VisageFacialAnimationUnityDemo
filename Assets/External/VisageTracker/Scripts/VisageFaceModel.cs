using PictoryGramAPI;
using PictoryGramAPI.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Visage.FaceTracking
{
	public class VisageFaceModel : MonoBehaviour
	{
		[SerializeField]
		private TextAsset configuration;

		[SerializeField]
		private List<ActionUnitBinding> actionUnitBindings = new List<ActionUnitBinding>();

		private Dictionary<string, SkinnedMeshRenderer> skinnedMeshRenderes = new Dictionary<string, SkinnedMeshRenderer>();

		private bool skinnedMeshRendererHasBSNamePrefix;

		private VisageTracker tracker;

		private BlendshapeRecorder blendshapeRecorder;

		private AudioRecorder audioRecorder;

		private string BlendshapeRecordingFilePath { get { return Path.Combine(Application.persistentDataPath, BlendshapeRecorder.RecordedFileName); } }

		private bool isSendingRecording;

		int frameIndex;


		string[,] recordings = new string[,] {
			{ "72c9326cfcca973c2e3cc130f32fe9a6.dat","a2c540413f896c63b04092e43d50dc64.dat" },
			{ "88c4db0cfe499c8432d195419ced99c2.dat","8a12f42d5b032599bfbec38649c00044.dat" },
			{ "8923e9962aff93285186ed875de06cbd.dat","9d7f17ee7658bb96f525e5f67c598aea.dat" },
			{ "53fd8b982a033a994bc153ff03dacedf.dat","a1c3a223c31a605c63f043f82d9f2f78.dat" },
			{ "4321a5d1854b59c4740894c62b1f1dc0.dat","fa9e7ca48cedf864461e5d17fbe0ece9.dat" },
			{ "8286f071b051b04e450103f346b3a068.dat","1b817ac7bb953dc79b096120f2e81ed7.dat" },
			{ "b9366ee8563ea447a5a15c9bfc34840f.dat","26a3cdc57cf67d21e3900117ba36c385.dat" },
			{ "92e5d82d680c916c558b7873e0de08f3.dat","bfda175ebb14eb709273188667a11e8f.dat" },
		};

		private void Start()
		{
			SetupBinding(VisageTracker.Instance);
			
			tracker.AddListenerToPlayButton(OnPlayingButtonClick);
			tracker.AddListenerToRecordingButton(OnRecordingButtonClick);
			tracker.AddListenerToSendRecordingButton(OnSendRecordingButtonClick);

			string headRendererName = "Head";
			SkinnedMeshRenderer headRenderer;
			if (skinnedMeshRenderes.ContainsKey(headRendererName))
			{
				headRenderer = skinnedMeshRenderes[headRendererName];

				blendshapeRecorder = new BlendshapeRecorder(actionUnitBindings);
				audioRecorder = new AudioRecorder(GetComponent<AudioSource>(), this);

				blendshapeRecorder.LoadBlenshapesRecording(Path.Combine(Application.persistentDataPath, BlendshapeRecorder.RecordedFileName));
				audioRecorder.LoadWavClip(Path.Combine(Application.persistentDataPath, AudioRecorder.RecordedFileName));

				//int recordingIndex = 0;

				//blendshapeRecorder.LoadBlenshapesRecording(Path.Combine(Application.persistentDataPath, recordings[recordingIndex,1]));
				//audioRecorder.LoadRecordedClip(Path.Combine(Application.persistentDataPath, recordings[recordingIndex, 0]));


				bool showSendRecordingButton = blendshapeRecorder.BlendshapesByteBuffer != null && blendshapeRecorder.BlendshapesByteBuffer.Length > 0;

				tracker.SetPlaying(false, showSendRecordingButton);
			}

			isSendingRecording = false;
		}

		private void OnPlayingButtonClick()
		{
			if (tracker.IsPlaying)
			{
				tracker.SetPlaying(false);
				audioRecorder.PausePlayingClip();
			}
			else if (blendshapeRecorder.RecordedFramesCount > 0)
			{
				tracker.SetPlaying(true);
				blendshapeRecorder.StartReplay();
				audioRecorder.PlayClip();
			}
			Debug.Log("isPlaying=" + tracker.IsPlaying + " blendshapeRecorder.RecordedFramesCount =" + blendshapeRecorder.RecordedFramesCount + " tracker.IsTracking=" + tracker.IsTracking);
			frameIndex = 0;
		}

		private void OnRecordingButtonClick()
		{
			string filename = "blendshapesRecording";
			string filepath = Path.Combine(Application.persistentDataPath, filename);

			if (tracker.IsRecording)
			{
				tracker.messageSendText.text = "Saving...";
				tracker.gameObject.SetActive(true);
				tracker.SetRecording(false);
				blendshapeRecorder.StopRecording();
				this.StartCoroutineActionAfterFrames(() => {
					audioRecorder.StopAndSaveRecording();
					blendshapeRecorder.SaveBlendshapesRecording(BlendshapeRecordingFilePath);
					blendshapeRecorder.LoadBlenshapesRecording(BlendshapeRecordingFilePath);
					tracker.messageSendText.gameObject.SetActive(false);
				}, 1);
			}
			else
			{
				blendshapeRecorder.StartRecording();
				audioRecorder.StartRecording();
				tracker.SetRecording(true);
			}
			frameIndex = 0;
			Debug.Log("isRecording=" + tracker.IsRecording);
		}

		private void OnSendRecordingButtonClick()
		{
			SendRecording();
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
			if (tracker.IsTracking)
			{
				foreach (var binding in actionUnitBindings)
				{
					binding.UpdateAction(blendshapeRecorder.blendshapeWeights);
				}
			}
			if (tracker.IsRecording)
			{
				if (!blendshapeRecorder.SaveBlendshapeWeights() || !tracker.IsTracking)
				{
					OnRecordingButtonClick();
				}
			}
			else if (tracker.IsPlaying)
			{
				if (blendshapeRecorder.LoadBlendsapeWeights())
				{
					foreach (var binding in actionUnitBindings)
					{
						binding.SetBlenshapeWeight(blendshapeRecorder.blendshapeWeights);
					}
				}
				else
				{
					tracker.SetPlaying(false);
					blendshapeRecorder.StopReplay();
					audioRecorder.StopPlayingClip();
					frameIndex = 0;
					Debug.Log("isPlaying=" + tracker.IsPlaying + " blendshapeRecorder.RecordedFramesCount =" + blendshapeRecorder.RecordedFramesCount + " tracker.IsTracking=" + tracker.IsTracking);
				}
			}

			frameIndex++;
		}

		private void SetupBinding(VisageTracker tracker)
		{
			actionUnitBindings.Clear();

			this.tracker = tracker;
			skinnedMeshRendererHasBSNamePrefix = true;
			skinnedMeshRenderes.Clear();
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (var renderer in renderers)
			{
				skinnedMeshRenderes.Add(renderer.name, renderer);
			}

			string text = configuration.text;
			string[] lines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			string debugLogText = "";
			foreach (string line in lines)
			{
				// skip comments
				if (line.StartsWith("#"))
					continue;

				string[] values = line.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				string[] trimmedValues = new string[8];
				for (int i = 0; i < Mathf.Min(values.Length, 8); i++)
				{
					// trim values
					trimmedValues[i] = values[i].Trim();
				}

				// parse au name
				string au = trimmedValues[0];

				// parse blendshape identifier
				string blendshape = trimmedValues[1];
				string[] blendShapeParts = blendshape.Split(':');
				if (blendShapeParts.Length < 2)
				{
					Debug.LogError("Invalid blendshape_indentifier value in configuration '" + configuration.name + "'.", tracker);
					return;
				}

				string blendshapeObjectName = blendShapeParts[0];
				string blendshapeName = blendShapeParts[1];
				string blendshapeNameWithPrefix = string.Concat(blendshapeObjectName, "_blendShape.", blendshapeName);
				//int blendshapeIndex = 0;
				//if (!int.TryParse(blendShapeParts[1], out blendshapeIndex))
				//{
				//	Debug.LogError("Invalid blendshape_indentifier value in configuration '" + configuration.name + "'.", tracker);
				//	return;
				//}

				//GameObject target = GameObject.Find(blendshapeObjectName);
				//if (target == null || target.GetComponent<SkinnedMeshRenderer>() == null)
				//{
				//	Debug.LogError(target == null);
				//	Debug.LogError("No valid blendshape target named '" + blendshapeObjectName + "' defined in configuration: '" + configuration.name + "'.", tracker);
				//	return;
				//}

				// parse min limit
				float min = -1f;
				if (!float.TryParse(trimmedValues[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out min))
				{
					Debug.LogError("Invalid min_limit value in binding configuration '" + configuration.name + "'.", tracker);
					return;
				}

				// parse max limit
				float max = 1f;
				if (!float.TryParse(trimmedValues[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out max))
				{
					Debug.LogError("Invalid max_limit value in binding configuration '" + configuration.name + "'.", tracker);
					return;
				}

				// parse inverted
				bool inverted = false;
				if (!string.IsNullOrEmpty(trimmedValues[4]))
					inverted = trimmedValues[4] == "1";

				// parse weight
				float weight = 1f;
				if (!string.IsNullOrEmpty(trimmedValues[5]))
				{
					if (!float.TryParse(trimmedValues[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out weight))
					{
						Debug.LogError("Invalid weight value in binding configuration '" + configuration.name + "'.", tracker);
						return;
					}
				}

				// parse filter window
				int filterWindow = 6;
				if (!string.IsNullOrEmpty(trimmedValues[6]))
				{
					if (!int.TryParse(trimmedValues[6], out filterWindow) || filterWindow < 0 || filterWindow > 16)
					{
						Debug.LogError("Invalid filter_window value in binding configuration '" + configuration.name + "'.", tracker);
						return;
					}
				}

				// parse filter amount
				float filterAmount = 0.3f;
				if (!string.IsNullOrEmpty(trimmedValues[7]))
				{
					if (!float.TryParse(trimmedValues[7], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out filterAmount))
					{
						Debug.LogError("Invalid filter_amount value in binding configuration '" + configuration.name + "'.", tracker);
						return;
					}
				}

				if (skinnedMeshRenderes.ContainsKey(blendshapeObjectName))
				{
					Debug.Log("blendshapeNameWithPrefix=" + blendshapeNameWithPrefix);
					SkinnedMeshRenderer renderer = skinnedMeshRenderes[blendshapeObjectName];   //GameObject.Find(blendshapeObjectName).GetComponent<SkinnedMeshRenderer>();
					int blendshapeIndex = renderer.sharedMesh.GetBlendShapeIndex(blendshapeNameWithPrefix);
					if (blendshapeIndex < 0)
					{
						blendshapeNameWithPrefix = String.Concat(blendshapeNameWithPrefix, " ");
						blendshapeIndex = renderer.sharedMesh.GetBlendShapeIndex(blendshapeNameWithPrefix);
					}
					if (blendshapeIndex >= 0)
					{
						// add new binding
						ActionUnitBinding binding = new ActionUnitBinding();// gameObject.AddComponent<ActionUnitBinding>();
						binding.Name = au + " -> " + blendshape;
						binding.Tracker = tracker;
						binding.ActionUnitName = au;
						binding.Limits = new Vector2(min, max);
						binding.Inverted = inverted;
						binding.Weight = weight;
						binding.FilterWindowSize = filterWindow;
						binding.FilterConstant = filterAmount;
						binding.Targets = new ActionUnitBindingTarget[1];
						binding.Targets[0] = new ActionUnitBindingTarget();
						binding.Targets[0].Renderer = renderer;
						binding.Targets[0].BlendshapeIndex = blendshapeIndex;
						binding.Targets[0].BlendshapeName = blendshapeName;
						binding.Targets[0].Weight = 1f;
						debugLogText += gameObject.name + ".SetupBinding : " + blendshapeObjectName + " : " + blendshapeName + " : " + blendshapeIndex + "\n";
						actionUnitBindings.Add(binding);
					}
					else
					{
						Debug.LogWarning(gameObject.name + ".SetupBinding : " + blendshapeObjectName + " : " + blendshapeName + " : " + blendshapeIndex);
					}
				}
				else
				{
					Debug.LogWarning(gameObject.name + ".SetupBinding: No such renderer: " + blendshapeObjectName + " : " + blendshapeName);
				}
			}
			Debug.Log(debugLogText);
			//SkinnedMeshRenderer eyelashesRenderer = GameObject.Find("Eyelashes").GetComponent<SkinnedMeshRenderer>();
			//string blendshapeName2 = eyelashesRenderer.sharedMesh.GetBlendShapeName(16);
			//Debug.Log("*"+ blendshapeName2+"*");
		}

		public void SendRecording()
		{
			Text messageSendText = tracker.messageSendText;
			byte[] blendshapesByteBuffer = blendshapeRecorder.BlendshapesByteBuffer;
			if (!isSendingRecording && blendshapesByteBuffer != null && blendshapesByteBuffer.Length > 0)
			{
				if (messageSendText != null)
				{
					messageSendText.gameObject.SetActive(true);
					messageSendText.text = "Sending...";
				}
				Debug.Log("SendRecording blendshapesByteBuffer.length=" + blendshapesByteBuffer.Length);
				isSendingRecording = true;
				BlendshapesRecordingMessage message = new BlendshapesRecordingMessage();
				message.BlendshapesRecording = new PictoryGramAPIFile(blendshapesByteBuffer);
				if (audioRecorder.RecordedClip != null && audioRecorder.RecordedClip.length > 0)
				{
					message.Recording = new PictoryGramAPIFile() { Data = AudioClipUtils.ToBytes(audioRecorder.RecordedClip) };
					message.RecordingIsCompressed = 0;
					Debug.Log("SendRecording audioRecorder.RecordedClip.length=" + audioRecorder.RecordedClip.length);
				}
				string userId = "b9af29e28b5c1203d447adce0c4bbaef";
				string auth = "b9af29e28b5c1203d447adce0c4bbaef6e748db181cb90b367721ca75da34806f1de7ae646754c9f80f09fcf69104bed0857d397defecb00914157c0a4edfa41";
				this.StartCoroutine(PictoryGramAPIFileUpload.SendFileRoutine(this, message, userId, auth, null, null, OnSendRecordingError, OnSendRecordingSuccess));
			}
		}

		private void OnSendRecordingError(string message)
		{
			Text messageSendText = tracker.messageSendText;
			if (messageSendText != null)
			{
				messageSendText.text = "Sending failed\n" + message;
				this.StartCoroutineActionAfterTime(() => {
					messageSendText.gameObject.SetActive(false);
				}, 4f);
			}
			isSendingRecording = false;
			Debug.Log("OnSendRecordingError " + message);
		}
		private void OnSendRecordingSuccess(Response<PictoryGramAPIObject> response)
		{
			Text messageSendText = tracker.messageSendText;
			isSendingRecording = false;
			messageSendText.text = "Recording sent!";
			this.StartCoroutineActionAfterTime(() => {
				messageSendText.gameObject.SetActive(false);
			}, 4f);
			Debug.Log("OnSendRecordingSuccess " + response.Data.Status);

		}
	}
}
