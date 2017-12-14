using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using PictoryGramAPI.Client;
using PictoryGramAPI;
using SimpleSQL;

/// <summary>
/// Default user class.
/// Its profile class contains default DataObject firlds.
/// If you want to have a custom user, you need to override AbstractUser and provide custom Profile class.
/// </summary>
namespace PictoryGramAPI.Data {
	
	public enum FriendStatusEnum
	{
		NOT_FRIEND,     // = 0
		ACTIVE_FRIEND,  // = 1
		BLOCKED_FRIEND, // = 2
        NOT_REGISTERED_FRIEND,   // = 3
	}

	public enum FriendTypeEnum
	{
		FACEBOOK, // = 0
		EMAIL, // = 1
		MOBILE, // = 2
	}

	public class Friend : BaseUser 
	{
		public const string FIELD_FRIEND_IMPORTED_ID = "importedFriendID";
        public const string FIELD_FRIEND_NAME = "name";
		public const string FIELD_FRIEND_STATUS = "status";
		public const string FIELD_FRIEND_VALUE = "value";
		public const string FIELD_FRIEND_TYPE = "type";

		[PrimaryKey, AutoIncrement] // SimpleSQL
		[JsonProperty(FIELD_FRIEND_IMPORTED_ID)]
		public int ImportedFriendId { get; set; }

		[JsonProperty(FIELD_FRIEND_STATUS)]
		public FriendStatusEnum FriendStatus { get; set; }

		[JsonProperty(FIELD_FRIEND_NAME)]
		public string Name { get; set; }

		[JsonProperty(FIELD_FRIEND_VALUE)]
		public string Value { get; set; }

		[JsonProperty(FIELD_FRIEND_TYPE)]
		public FriendTypeEnum Type { get; set; }

		public Friend() { }
	}

}