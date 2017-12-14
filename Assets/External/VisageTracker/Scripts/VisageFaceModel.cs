using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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

		private string BlendshapeRecordingFilePath { get { return Path.Combine(Application.persistentDataPath, "blendshapeRecording"); } }

		int frameIndex;

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
				blendshapeRecorder = new BlendshapeRecorder(actionUnitBindings, headRenderer, tracker.messageSendText);
				blendshapeRecorder.LoadBlenshapesRecording(Path.Combine(Application.persistentDataPath, "c964f7bee92fee6d57325b3696725099.dat"));
				tracker.SetPlaying(false);
			}
		}

		private void OnPlayingButtonClick()
		{
			if (tracker.IsPlaying)
			{
				tracker.SetPlaying(false);
			}
			else if (blendshapeRecorder.RecordedFramesCount > 0)
			{
				tracker.SetPlaying(true);
				blendshapeRecorder.StartReplay();
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
				blendshapeRecorder.StopRecording();
				blendshapeRecorder.SaveBlendshapesRecording(BlendshapeRecordingFilePath);
				blendshapeRecorder.LoadBlenshapesRecording(BlendshapeRecordingFilePath);
				tracker.SetRecording(false);
			}
			else
			{
				blendshapeRecorder.StartRecording();
				tracker.SetRecording(true);
			}
			frameIndex = 0;
			Debug.Log("isRecording=" + tracker.IsRecording);
		}

		private void OnSendRecordingButtonClick()
		{
			blendshapeRecorder.SendRecording(this);
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
				if (tracker.IsRecording)
				{
					if (!blendshapeRecorder.SaveBlendshapeWeights())
					{
						OnRecordingButtonClick();
					}
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
					OnPlayingButtonClick();
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

	}
}
