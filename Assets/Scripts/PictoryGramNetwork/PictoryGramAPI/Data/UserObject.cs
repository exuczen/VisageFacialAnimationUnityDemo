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
	
    public class UserObject : PictoryGramAPIObject
    {
        public const string FIELD_USER   = "user";

        [JsonProperty(FIELD_USER, NullValueHandling = NullValueHandling.Ignore)]
        public User user { get; set; }

        public UserObject()
		{
		}
    }
}