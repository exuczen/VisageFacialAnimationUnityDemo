using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PictoryGramAPI.Client;
using PictoryGramAPI;
using PictoryGramAPI.Data;

/// <summary>
/// Default Messages class.
/// Its profile class contains default DataObject fields.
/// If you want to have a custom Messages, you need to override AbstractMessages and provide custom Profile class.
/// </summary>
namespace PictoryGramAPI.Data {
	public class PictorygramCharacters<T> where T : PictoryGramAPIObject, new() {

		public PictorygramCharacters() { }

		public Coroutine ListCharacters(Action<Response<CharactersObject>> onSuccess, Action<Response<CharactersObject>> onError, string characterId = null, int limit = 50, bool direction = false)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=listCharacters", onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "characters");
		}

		public Coroutine ListUserCharacters(Action<Response<UserCharactersObject>> onSuccess, Action<Response<UserCharactersObject>> onError, /*string userId, string auth,*/ string characterId, int limit = 50, bool direction = false)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=listUserCharacters", onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "characters?" /*+ "userId=" + userId  + "&auth=" + auth*/ + "characterId=" + characterId);
		}
	}
}