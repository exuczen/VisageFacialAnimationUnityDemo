using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipUtils
{
    public static byte[] ToBytes(AudioClip clip)
    {
        SerializableAudioClip serializable = new SerializableAudioClip();
        serializable.channels = clip.channels;
        serializable.frequency = clip.frequency;
        serializable.lengthSamples = clip.samples;

        float[] recordingBytes = new float[clip.samples * clip.channels];
        clip.GetData (recordingBytes, 0);

        serializable.bytes = new byte[recordingBytes.Length * 4];
        System.Buffer.BlockCopy(recordingBytes, 0, serializable.bytes , 0, serializable.bytes.Length);

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(serializable);
        return System.Text.Encoding.ASCII.GetBytes(json);
    }

    public static AudioClip ToAudio(byte[] bytes)
    {
        string json = System.Text.Encoding.ASCII.GetString(bytes);
        SerializableAudioClip serializable = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializableAudioClip>(json);
        AudioClip clip = AudioClip.Create("clip", serializable.lengthSamples, serializable.channels, serializable.frequency, false);

        float[] recordingBytes = new float[serializable.bytes.Length / 4];
        System.Buffer.BlockCopy(serializable.bytes, 0, recordingBytes , 0, serializable.bytes.Length);
        clip.SetData(recordingBytes, 0);
        return clip;
    }

    private struct SerializableAudioClip
    {
        public int channels;
        public int frequency;
        public int lengthSamples;
        public byte[] bytes;
    }
}
