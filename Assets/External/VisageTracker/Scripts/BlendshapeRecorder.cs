using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using PictoryGramAPI.Data;
using PictoryGramAPI;
using UnityEngine.UI;

namespace Visage.FaceTracking
{
	public class BlendshapeRecorder
	{
		private SkinnedMeshRenderer headRenderer;

		public Dictionary<string, byte> blendshapeWeights;

		private Dictionary<string, byte[]> recordedBlendshapes;

		private Dictionary<string, byte[]> blendshapeNameByteArrays;

		private const int singleBlendshapeCapacity = 30 * 30 + 100;

		private int recordedFramesCount;

		private int frameIndex;

		private byte[] blendshapesByteBuffer;

		private Text messageSendText;

		public int RecordedFramesCount { get { return recordedFramesCount; } }

		private bool isSendingRecording;

		private MonoBehaviour messageSendingContext;

		public BlendshapeRecorder()
		{

		}

		public BlendshapeRecorder(List<ActionUnitBinding> actionUnitBindings, SkinnedMeshRenderer customRenderer, Text messageSentText)
		{
			headRenderer = customRenderer;

			isSendingRecording = false;

			recordedFramesCount = 0;
			frameIndex = 0;

			blendshapeWeights = new Dictionary<string, byte>();
			recordedBlendshapes = new Dictionary<string, byte[]>();
			blendshapeNameByteArrays = new Dictionary<string, byte[]>();

			foreach (var binding in actionUnitBindings)
			{
				ActionUnitBindingTarget target = binding.Targets[0];
				SkinnedMeshRenderer renderer = target.Renderer;
				string blendshapeName = target.BlendshapeName;
				if (!blendshapeWeights.ContainsKey(blendshapeName))
				{
					blendshapeNameByteArrays.Add(blendshapeName, Encoding.ASCII.GetBytes(blendshapeName));
					blendshapeWeights.Add(blendshapeName, 0);
					recordedBlendshapes.Add(blendshapeName, new byte[singleBlendshapeCapacity]);
				}
			}

			this.messageSendText = messageSentText;
		}

		public void StartRecording()
		{
			frameIndex = 0;
			recordedFramesCount = 0;
			Dictionary<string, byte[]>.KeyCollection asd = recordedBlendshapes.Keys;
			for (int i = 0; i < recordedBlendshapes.Count; i++)
			{
				string blendshapeName = recordedBlendshapes.ElementAt(i).Key;
				recordedBlendshapes[blendshapeName] = new byte[singleBlendshapeCapacity];
				blendshapeWeights[blendshapeName] = 0;
			}
		}

		public void StopRecording()
		{
			frameIndex = 0;
		}


		public void StartReplay()
		{
		}

		public bool LoadBlendsapeWeights()
		{
			//Debug.Log("LoadBlendsapeWeights: " + frameIndex + " " + recordedFramesCount);
			if (frameIndex < recordedFramesCount)
			{

				foreach (var recordedBlenshape in recordedBlendshapes)
				{
					byte[] recordedBlendshapeFrames = recordedBlenshape.Value;
					byte blenshapeValue = recordedBlendshapeFrames[frameIndex];
					blendshapeWeights[recordedBlenshape.Key] = blenshapeValue;
				}
				frameIndex++;
				return true;
			}
			frameIndex = 0;
			return false;
		}

		public bool SaveBlendshapeWeights()
		{
			if (frameIndex < singleBlendshapeCapacity)
			{
				recordedFramesCount = frameIndex;
				foreach (var blendshapeWeight in blendshapeWeights)
				{
					byte[] recordedBlendshapeFrames = recordedBlendshapes[blendshapeWeight.Key];
					recordedBlendshapeFrames[frameIndex] = blendshapeWeight.Value;
				}
				frameIndex++;
				return true;
			}
			recordedFramesCount = singleBlendshapeCapacity;
			frameIndex = 0;
			return false;
		}

		public void SaveBlendshapesRecording(string filepath)
		{
			int blendshapesCount = recordedBlendshapes.Count;

			int bytesCounter = 4 + 4 + 4;
			foreach (var recording in recordedBlendshapes)
			{
				bytesCounter += (4 + blendshapeNameByteArrays[recording.Key].Length + singleBlendshapeCapacity);
			}
			blendshapesByteBuffer = new byte[bytesCounter];


			Debug.Log("SaveBlendshapesRecording: " + blendshapesCount + " " + singleBlendshapeCapacity + " " + recordedFramesCount);

			int offset = 0;
			Buffer.BlockCopy(BitConverter.GetBytes(blendshapesCount), 0, blendshapesByteBuffer, offset, 4);
			offset += 4;
			Buffer.BlockCopy(BitConverter.GetBytes(singleBlendshapeCapacity), 0, blendshapesByteBuffer, offset, 4);
			offset += 4;
			Buffer.BlockCopy(BitConverter.GetBytes(recordedFramesCount), 0, blendshapesByteBuffer, offset, 4);
			offset += 4;

			foreach (var recording in recordedBlendshapes)
			{
				string blendshapeName = recording.Key;
				byte[] blenshapeFrames = recording.Value;
				byte[] blendshapeNameBytes = blendshapeNameByteArrays[blendshapeName];
				byte[] blendshapeNameLengthBytes = BitConverter.GetBytes(blendshapeNameBytes.Length);

				Buffer.BlockCopy(blendshapeNameLengthBytes, 0, blendshapesByteBuffer, offset, 4);
				offset += 4;
				Buffer.BlockCopy(blendshapeNameBytes, 0, blendshapesByteBuffer, offset, blendshapeNameBytes.Length);
				offset += blendshapeNameBytes.Length;
				Buffer.BlockCopy(blenshapeFrames, 0, blendshapesByteBuffer, offset, singleBlendshapeCapacity);
				offset += singleBlendshapeCapacity;
			}
			File.WriteAllBytes(filepath, blendshapesByteBuffer);
		}

		public void LoadBlenshapesRecording(string filepath)
		{
			if (!File.Exists(filepath))
				return;

			blendshapeWeights = new Dictionary<string, byte>();
			recordedBlendshapes = new Dictionary<string, byte[]>();

			blendshapesByteBuffer = File.ReadAllBytes(filepath);

			blendshapeWeights = new Dictionary<string, byte>();
			int offset = 0;
			int blendshapesCount = BitConverter.ToInt32(blendshapesByteBuffer, offset);
			offset += 4;
			int singleBlendshapeCapacity = BitConverter.ToInt32(blendshapesByteBuffer, offset);
			offset += 4;
			recordedFramesCount = BitConverter.ToInt32(blendshapesByteBuffer, offset);
			offset += 4;
			Debug.Log("LoadBlenshapesRecording: " + blendshapesCount + " " + singleBlendshapeCapacity + " " + recordedFramesCount);
			for (int i = 0; i < blendshapesCount; i++)
			{
				int blendshapeNameLength = BitConverter.ToInt32(blendshapesByteBuffer, offset);
				offset += 4;
				string blendshapeName = Encoding.ASCII.GetString(blendshapesByteBuffer, offset, blendshapeNameLength);
				offset += blendshapeNameLength;
				byte[] destByteArray = new byte[singleBlendshapeCapacity];
				Buffer.BlockCopy(blendshapesByteBuffer, offset, destByteArray, 0, singleBlendshapeCapacity);
				//Debug.Log("LoadBlenshapesRecording: blendshapeName=" + blendshapeName);
				recordedBlendshapes.Add(blendshapeName, destByteArray);
				blendshapeWeights.Add(blendshapeName, 0);
				offset += singleBlendshapeCapacity;
			}
		}


		public void SendRecording(MonoBehaviour context)
		{
			this.messageSendingContext = context;
			if (!isSendingRecording && blendshapesByteBuffer != null && blendshapesByteBuffer.Length > 0)
			{
				if (messageSendText != null)
				{
					messageSendText.gameObject.SetActive(true);
					messageSendText.text = "Sending...";
				}
				Debug.Log("SendRecording");
				isSendingRecording = true;
				BlendshapesRecordingMessage message = new BlendshapesRecordingMessage();
				message.Recording = new PictoryGramAPIFile(blendshapesByteBuffer);
				string userId = "b9af29e28b5c1203d447adce0c4bbaef";
				string auth = "b9af29e28b5c1203d447adce0c4bbaef6e748db181cb90b367721ca75da34806f1de7ae646754c9f80f09fcf69104bed0857d397defecb00914157c0a4edfa41";
				context.StartCoroutine(PictoryGramAPIFileUpload.SendFileRoutine(context, message, userId, auth, null, null, OnSendRecordingError, OnSendRecordingSuccess));
			}
		}

		private void OnSendRecordingError(string message)
		{
			if (messageSendingContext != null && messageSendText != null)
			{
				messageSendText.text = "Sending failed\n" + message;
				messageSendingContext.StartCoroutineActionAfterTime(() => {
					messageSendText.gameObject.SetActive(false);
				}, 4f);
			}
			isSendingRecording = false;
			Debug.Log("OnSendRecordingError " + message);
		}
		private void OnSendRecordingSuccess(Response<PictoryGramAPIObject> response)
		{
			isSendingRecording = false;
			messageSendText.text = "Recording sent!";
			messageSendingContext.StartCoroutineActionAfterTime(() => {
				messageSendText.gameObject.SetActive(false);
			}, 4f);
			Debug.Log("OnSendRecordingSuccess " + response.Data.Status);

		}
	}
}
