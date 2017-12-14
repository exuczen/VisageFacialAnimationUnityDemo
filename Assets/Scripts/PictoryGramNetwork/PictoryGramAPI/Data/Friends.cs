using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PictoryGramAPI.Client;
using PictoryGramAPI;
using PictoryGramAPI.Data;

namespace PictoryGramAPI.Data {
	
	public class Friends : PictoryGramAPIObject {
		public Friends () { }

		public Coroutine ListFriends(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string userId = null, int limit = PictoryGramAPIConstants.NUMBER_OF_DOWNLOADED_FRIENDS, bool direction = false)
		{
			string requestParams = "";
			if(userId != null)
				requestParams += "&userFriendId=" + userId;
			if(limit > 0)
				requestParams += "&limit=" + limit;
			if (direction)
				requestParams += "&direction=1";
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=listFriends"+requestParams, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/");
		}

		public Coroutine ListImportedFriends(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, long friendId = 0, int limit = PictoryGramAPIConstants.NUMBER_OF_DOWNLOADED_FRIENDS, bool direction = false)
		{
			string requestParams = "";
			if(friendId != 0)
				requestParams += "&importedFriendId=" + friendId;
			if(limit > 0)
				requestParams += "&limit=" + limit;
			if (direction)
				requestParams += "&direction=1";
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=listImportedFriends"+requestParams, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/");
		}

		public Coroutine RemoveImportedFriends(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string userId, string auth)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=removeImportedFriends&ID=" + userId + "&auth=" + auth, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/");
		}

		public Coroutine AddUser(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string friendId)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=addUser&friendId="+ friendId, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/");
		}

		public Coroutine AddFriend(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string friendContactValue, int friendContactType)
        {
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=addFriend&friendContactValue="+ friendContactValue + "&friendContactType=" + friendContactType, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/");
        }

        public Coroutine BlockFriend(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string friendID)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=blockFriend&friendId="+friendID, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/");
		}

		public Coroutine UnblockFriend(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string friendID)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=unblockFriend&friendId="+friendID, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/");
		}

		public Coroutine ImportContacts(Action<Response<FriendsObject>> onSuccess, Action<Response<FriendsObject>> onFailure, string postData)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=importContacts"+postData, onSuccess, onFailure, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/");
		}
	}
}

