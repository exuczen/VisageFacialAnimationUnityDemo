using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Visage.FaceTracking
{
	public class AudioRecorder
	{
		/// <summary>
		/// Recorded audio clip to send.
		/// </summary>
		private AudioClip recordedClip = null;

		/// <summary>
		/// Audio clip buffer for microphone.
		/// </summary>
		private AudioClip recordedBuffer;

		/// <summary>
		/// Prevents from starting the recording when it's already in progress and from stopping it if it's not in progress.
		/// </summary>
		private bool isRecording = false;

		/// <summary>
		/// Prevents from replaying the message with character animation if it's already playing and from stopping it if it's not playing.
		/// </summary>
		private bool isPlaying = false;

		/// <summary>
		/// The name of the first identified microphone device, used as an identifier to start recording with this device.
		/// </summary>
		string microphone;

		private MonoBehaviour context;

		private AudioSource audioSource;

		public const float MAX_RECORDING_DURATION = 30f;

		public const string recordedFileName = "recordedVoice.wav";

		public AudioClip RecordedClip { get { return recordedClip; } }

		public AudioRecorder(AudioSource audioSource, MonoBehaviour context)
		{
			this.audioSource = audioSource;
			this.context = context;
			microphone = (Microphone.devices != null && Microphone.devices.Length > 0) ? Microphone.devices[0] : null;
		}


		/// <summary>
		/// Replays recorded audio clip integrated with salsa character animation.
		/// </summary>
		public void PlayClip()
		{
			if (recordedClip != null && !isPlaying)
			{
				isPlaying = true;
				audioSource.clip = recordedClip;
				audioSource.Play();
			}
		}

		/// <summary>
		/// Stops playing audio clip and salsa animation.
		/// </summary>
		public void PausePlayingClip()
		{
			isPlaying = false;
			if (audioSource.isPlaying)
				audioSource.Pause();
		}

		/// <summary>
		/// Stops recording the message, destroys previous recorded clip if exists and replaces it with new one.
		/// </summary>
		public void StopRecording()
		{
			if (isRecording)
			{
				isRecording = false;

				int micPosition = Microphone.GetPosition(microphone);
				Microphone.End(microphone);

				context.StartCoroutineActionAfterTime(() => {
					DestroyRecordedClip();

					recordedClip = TrimAudioClip(recordedBuffer, micPosition);
					// Saving recoreded voice to wav - file needed to create video file
					if (recordedClip != null && recordedClip.length > 0)
					{
						bool audioFileCreated = SavWav.Save(recordedFileName, recordedClip);
					}
					if (recordedBuffer != null)
						AudioClip.Destroy(recordedBuffer);
					recordedBuffer = null;
				}, 0.1f);
			}
		}

		/// <summary>
		/// Starts recording the message to the audio clip buffer using microphone device with given name-identifier.
		/// </summary>
		public void StartRecording()
		{
			PausePlayingClip();

			if (!isRecording && microphone != null)
			{
				isRecording = true;
				//context.StartCoroutine(RecordingTimer(MAX_RECORDING_DURATION));

				// Record to buffer.
				recordedBuffer = Microphone.Start(microphone, true, 5 * 60, 44100);
			}
		}

		/// <summary>
		/// Trims audio clip to the given position in samples of the recording.
		/// </summary>
		/// <param name="clipToTrim"></param>
		/// <param name="trimPosition"></param>
		/// <returns></returns>
		private AudioClip TrimAudioClip(AudioClip clipToTrim, int trimPosition)
		{
			if (clipToTrim != null && clipToTrim.length > 0 && trimPosition > 0)
			{
				float[] soundData = new float[clipToTrim.samples * clipToTrim.channels];
				clipToTrim.GetData(soundData, 0);

				//Create shortened array for the data that was used for recording
				float[] newData = new float[trimPosition * clipToTrim.channels];

				//Copy the used samples to a new array
				for (int i = 0; i < newData.Length; i++)
				{
					newData[i] = soundData[i];
				}

				AudioClip clip = AudioClip.Create(clipToTrim.name, trimPosition, clipToTrim.channels, clipToTrim.frequency, false);
				clip.SetData(newData, 0);
				return clip;
			}
			else
			{
				return clipToTrim;
			}
		}

		//IEnumerator RecordingTimer(float duration)
		//{
		//	while ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && duration > 0 && isRecording) // First finger on touch devices acts like mouse.
		//	{
		//		duration -= Time.deltaTime;
		//		yield return null;
		//	}
		//	if (isRecording)
		//	{
		//		StopRecording();
		//	}
		//}

		public void DestroyRecordedClip()
		{
			if (recordedClip != null)
			{
				AudioClip.Destroy(recordedClip);
				recordedClip = null;
			}
		}

		public void LoadRecordedClip(string filepath)
		{
			//context.StartCoroutine(LoadRecordedClipRoutine(filepath));
			byte[] audioBytes = File.ReadAllBytes(filepath);
			recordedClip = AudioClipUtils.ToAudio(audioBytes);
		}

		private IEnumerator LoadRecordedClipRoutine(string filepath)
		{
			FileInfo fileInfo = new FileInfo(filepath);

			Debug.Log("LoadRecordedClip: fileCreated=" + fileInfo.Exists + " fileLength=" + fileInfo.Length);

			if (fileInfo.Exists)
			{
				WWW www = new WWW("file:///" + filepath);
				yield return new WaitUntil(() => www.isDone);
				yield return new WaitUntil(() => !isPlaying);
				AudioClip.Destroy(recordedClip);
				recordedClip = WWWAudioExtensions.GetAudioClip(www);
				Debug.Log("LoadRecordedClip: recorded clip: " + recordedClip);
			}
		}

	}
}
