using UnityEngine;
using System;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using PictoryGramAPI;
using PictoryGramAPI.Data;
using PictoryGramAPI.Request;
using Newtonsoft.Json;
using Newtonsoft;

public class PictoryGramAPIFileUpload
{

	public static IEnumerator SendFileRoutine(MonoBehaviour context, BlendshapesRecordingMessage message, string userId, string userAuth, List<string> recipientList, Action<float> onProgressCallback = null, Action<string> onError = null, Action<Response<PictoryGramAPIObject>> onDone = null)
    {
		string url = "http://pictorygramDev.pixzell.pl/json/face";
		//string url = PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "messages/";

		WWWForm wwwForm = new WWWForm();

        PropertyInfo[] properties = message.GetType().GetProperties();

        if(properties != null && properties.Length > 0)
        {
            for (int i = 0; i < properties.Length; i++) {

                PropertyInfo property = properties[i];

                object value = property.GetValue(message, null);
            }
        }

        if(message != null)
        {
            properties = message.GetType().GetProperties();

			if(properties != null && properties.Length > 0) {
                for (int i = 0; i < properties.Length ; i++) {

                    PropertyInfo property = properties[i];

                    object value = property.GetValue(message, null);

                    if(value != null)
                    {
                        JsonPropertyAttribute[] propertyAttritbues = property.GetCustomAttributes(typeof(JsonPropertyAttribute), true) as JsonPropertyAttribute[];
                        string propertyName = property.Name;

                        if(propertyAttritbues != null && propertyAttritbues.Length > 0)
                        {
                            JsonPropertyAttribute attribute = propertyAttritbues[0];
                            if(string.IsNullOrEmpty(attribute.PropertyName) == false)
                            {
                                propertyName = attribute.PropertyName;
                            }
                        }

                        if(value is PictoryGramAPIFile)
                        {
                            PictoryGramAPIFile PictoryGramAPIFile = value as PictoryGramAPIFile;

                            if(PictoryGramAPIFile.Data != null)
                            {
                                wwwForm.AddBinaryData(propertyName, PictoryGramAPIFile.Data);
                            }
                        }
                        else
                        {
                            wwwForm.AddField(propertyName, value.ToString());
                        }
                    }
                }
            }
			int UserCharacterPropertiesCount = (message.UserCharacterProperties == null) ? 0 : message.UserCharacterProperties.Count;
			for (int i = 0; i < UserCharacterPropertiesCount; i++)
			{
				UserCharacterProperty ucp = message.UserCharacterProperties [i];
				wwwForm.AddField("characterParameters[]", ucp.ParameterName);
				wwwForm.AddField("characterValues[]", ucp.ParameterValue.ToString());
			}
		}

        wwwForm.AddField ("auth", userAuth);
        wwwForm.AddField ("userId", userId);
        wwwForm.AddField ("method", "addFaceRecording");

        //for (int i = 0; i < recipientList.Count; i++)
        //{
        //    wwwForm.AddField("recipients[]", recipientList[i]);
        //}


        Dictionary<string,string> headerDisc = wwwForm.headers;

        WWW www = new WWW (url, wwwForm.data, headerDisc);

		// Update progress with every frame.
		if (onProgressCallback != null)
		{
			while (/*!www.isDone*/www.uploadProgress < 1f)
			{
				onProgressCallback(www.uploadProgress);
				yield return null;
			}
	//		onProgressCallback(www.uploadProgress);
		}

        yield return www;
		bool isHttpResponseOk = false;
		foreach (var header in www.responseHeaders) {
			if (header.Key.Equals("STATUS")) {
				if (!header.Value.Equals("HTTP/1.1 200 OK")) {
					if (onError != null)
						onError("500");
				} else {
					isHttpResponseOk = true;
				}
			}
		}
		if (isHttpResponseOk) {
			Debug.Log("www.text" + www.text);
			try {
				Response<PictoryGramAPIObject> apiObject = new Response<PictoryGramAPIObject> ();
				apiObject.SetData(www.text);
				if (apiObject.Data.Status != "OK") {
					if (onError != null)
						onError(apiObject.Data.Error);
				} else {
					Debug.Log("www.text" + www.text);
					if(onDone != null)
						onDone(apiObject);
				}
			} catch {
				if (onError != null)
					onError("500");
			}
		}
    }
}
