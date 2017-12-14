using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PictoryGramAPI.Client;
using PictoryGramAPI;

/// <summary>
/// Default user class.
/// Its profile class contains default DataObject firlds.
/// If you want to have a custom user, you need to override AbstractUser and provide custom Profile class.
/// </summary>
namespace PictoryGramAPI.Data {
    public class Users : PictoryGramAPIObject
    {
        public Users()
        {

        }

        public Coroutine Register(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string email, string password)
		{
            return PictoryGramAPIHttpClient.Instance.PostAsync("method=register&email=" + email + "&password=" + password, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/",null);
		}

        public Coroutine Login(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string email, string password)
		{
            return PictoryGramAPIHttpClient.Instance.PostAsync("method=login&email=" + email + "&password=" + password, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/", onAfterSuccess:OnAfterSuccess);
		}

        public Coroutine ResetPassword(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string email)
        {
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=resetPassword&email=" + email, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/");
        }

        public Coroutine SendActivationEmail(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string email)
        {
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=sendActivationEmail&email=" + email, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/");
        }

        public Coroutine FbLogin(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string accessToken)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=fbLogin&accessToken="+accessToken, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/", onAfterSuccess:OnAfterSuccess);
		}

        public Coroutine UpdateDeviceFirebaseToken(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string deviceToken)
		{
			string deviceType = "";
			#if UNITY_IOS	
				deviceType = "I";
			#endif
			#if UNITY_ANDROID	
				deviceType = "A";
			#endif

			return PictoryGramAPIHttpClient.Instance.PostAsync("method=updateDeviceToken&deviceToken="+deviceToken+"&deviceType="+deviceType, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/", onAfterSuccess:OnAfterSuccess);
		}

        public Coroutine UpdateUserData(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, User user)
		{
            string userData = "&names[]=username&values[]="+ user.UserName + "&names[]=firstname&values[]=" + user.FirstName + "&names[]=surname&values[]=" + user.Surname + "&names[]=mobile&values[]=" + user.Mobile;
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=updateUserData"+userData, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/", onAfterSuccess:null);
		}

		public Coroutine UpdateAvatar(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, User user, String imageEncode64)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=updateAvatar&image=" + imageEncode64, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/", onAfterSuccess: null);
		}

		public Coroutine importFB(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string accessToken)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=importFb&accessToken="+accessToken, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/", null);
		}

        public Coroutine importGmail(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string accessToken)
		{
			return PictoryGramAPIHttpClient.Instance.PostAsync("method=importGmail&accessToken="+accessToken, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL +"users/", onAfterSuccess:null);
		}

        public Coroutine ValidateUserName(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string userName)
        {
            return PictoryGramAPIHttpClient.Instance.PostAsync("method=validateUsername&username=" + userName, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/", null);
        }

        public Coroutine ChangePassword(Action<Response<UserObject>> onSuccess, Action<Response<UserObject>> onError, string password, string newPassword)
        {
            return PictoryGramAPIHttpClient.Instance.PostAsync("method=changePassword&password=" + password + "&newPassword=" + newPassword, onSuccess, onError, null, PictoryGramAPIConstants.PRODUCTION_SERVER_URL + "users/", null);
        }

        private void OnAfterSuccess(string response)
		{
			Debug.Log("Users.OnAfterSuccess: Is this required?");
			Debug.Log("Users.OnAfterSuccess: response=" + response);
			if (!string.IsNullOrEmpty(response))
			{
				UserObject userObject = new UserObject();
				JsonConvert.PopulateObject(response, userObject);
				if (userObject.user != null)
				{
					PictoryGramAPIClient.Instance.UserID = userObject.user.UserId;
					PictoryGramAPIClient.Instance.UserKey = userObject.user.Auth;
				}
			}
		}
	}
}