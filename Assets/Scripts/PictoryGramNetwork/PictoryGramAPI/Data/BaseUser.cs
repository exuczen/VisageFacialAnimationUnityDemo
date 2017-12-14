using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PictoryGramAPI.Client;
using PictoryGramAPI;
using SimpleSQL;

/// <summary>
/// Common user part for all kind of users.
/// </summary>
namespace PictoryGramAPI.Data {
	
	public abstract class BaseUser
    {
        public const string FIELD_USER_ID   = "userId";
        public const string FIELD_FIRST_NAME = "firstname";
        public const string FIELD_SURNAME   = "surname";
        public const string FIELD_AVATAR    = "avatar";

//		[PrimaryKey] // SimpleSQL
		[JsonProperty(FIELD_USER_ID, NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty(FIELD_FIRST_NAME)]
        public string FirstName { get; set; }

        [JsonProperty(FIELD_SURNAME)]
        public string Surname { get; set; }

        [JsonProperty(FIELD_AVATAR)]
        public string Avatar { get; set; }

        public BaseUser ()
        {
        }
    }
}