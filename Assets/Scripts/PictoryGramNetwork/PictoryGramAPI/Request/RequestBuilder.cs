using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PictoryGramAPI.Client;
using PictoryGramAPI.Data;

namespace PictoryGramAPI.Request {
	
	/// <summary>
	/// This class is a gate for making call to PictoryGramAPI between client and the library.
	/// </summary>
	public class RequestBuilder {

		/// <summary>
		/// Post the specified url, obj, onSuccess and onError.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="obj">Object.</param>
		/// <param name="onSuccess">On success.</param>
		/// <param name="onError">On Error.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public Coroutine Post<T>(string url, string parameters, Action<Response<T>> onSuccess, Action<Response<T>> onError)  where T : PictoryGramAPIObject, new() {
			return PictoryGramAPIHttpClient.Instance.PostAsync(parameters, onSuccess, onError, url:url);
		}

			
	
		private void CheckCallbacks<T>(Delegate onSuccess, Delegate onError) where T : PictoryGramAPIObject, new()
		{
			if(onSuccess == null || onError == null)
			{
				throw new Exception("Callbacks cannot be null." + "onSuccess == null" + (onSuccess == null).ToString() + " onError == " + (onError == null).ToString());
			}
		}
	}
}