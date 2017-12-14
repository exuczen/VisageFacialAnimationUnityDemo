using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PictoryGramAPI.Client;
using PictoryGramAPI;

/// <summary>
/// User with all details. Used to represent logged in user.
/// </summary>
namespace PictoryGramAPI.Data {
	
    public class User: BaseUser
    {
        public const string FIELD_AUTH = "auth";
        public const string FIELD_STATUS = "status";
        public const string FIELD_EMAIL = "email";
        public const string FIELD_MOBILE = "mobile";
        public const string FIELD_FIREBASE_TOKEN = "firebaseToken";
        public const string FIELD_USERNAME = "username";


        public enum UserStatusEnum
        {
            DELETED = 0,
            ACTIVATED = 1,
            NOT_ACTIVATED = 2,
        }

        [JsonProperty(FIELD_AUTH)]
        public string Auth { get; set; }

        [JsonProperty(FIELD_USERNAME)]
        public string UserName { get; set; }

        [JsonProperty(FIELD_STATUS)]
        public UserStatusEnum Status { get; set; }

        [JsonProperty(FIELD_EMAIL)]
        public string Email { get; set; }

        [JsonProperty(FIELD_MOBILE)]
        public string Mobile { get; set; }

        [JsonProperty(FIELD_FIREBASE_TOKEN)]
        public string FirebaseToken { get; set; }

        public User ()
        {
        }
    }
}