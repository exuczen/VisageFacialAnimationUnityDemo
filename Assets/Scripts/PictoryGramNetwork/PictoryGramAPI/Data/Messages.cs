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
/// Its profile class contains default DataObject firlds.
/// If you want to have a custom Messages, you need to override AbstractMessages and provide custom Profile class.
/// </summary>
namespace PictoryGramAPI.Data {
	public class Messages<T> where T : PictoryGramAPIObject, new() {

		public Messages() { }

		public Coroutine ListMessages(Action<Response<MessagesObject>> onSuccess, Action<Response<MessagesObject>> onError, string messageId = null, int limit = PictoryGramAPIConstants.NUMBER_OF_DOWNLOADED_MESSAGES, bool direction = false)
		{
			string requestParams = "";
			if(!string.IsNullOrEmpty(messageId))
				requestParams += "&messageId=" + messageId;
			if(limit > 0)
				requestParams += "&limit=" + limit;
			if (direction)
				requestParams += "&direction=1";
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=listMessages"+requestParams, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"messages");
		}

		public Coroutine ListSentMessages(Action<Response<MessagesObject>> onSuccess, Action<Response<MessagesObject>> onError, string messageId = null, int limit = PictoryGramAPIConstants.NUMBER_OF_DOWNLOADED_MESSAGES, bool direction = false)
        {
			string requestParams = "";
			if(!string.IsNullOrEmpty(messageId))
				requestParams += "&messageId=" + messageId;
			if(limit > 0)
				requestParams += "&limit=" + limit;
			if (direction)
				requestParams += "&direction=1";
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=listSentMessages"+requestParams, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"messages");
        }

		public Coroutine RemoveMessage(Action<Response<MessagesObject>> onSuccess, Action<Response<MessagesObject>> onError, string messageID)
		{
            return PictoryGramAPIHttpClient.Instance.PostAsync("method=removeMessage&messageId="+messageID, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"messages");
		}

		public Coroutine ReadMessage(Action<Response<MessagesObject>> onSuccess, Action<Response<MessagesObject>> onError, string messageID)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=readMessage&messageId="+messageID, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"messages");
		}
	}
}