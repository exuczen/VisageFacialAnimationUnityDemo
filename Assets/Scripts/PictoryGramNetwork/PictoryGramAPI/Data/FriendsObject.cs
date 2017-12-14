using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PictoryGramAPI.Request;
using Newtonsoft.Json;
using PictoryGramAPI;

namespace PictoryGramAPI.Data {
	/// <summary>
	/// Class representing Friends response from backend.
	/// </summary>
	public class FriendsObject : PictoryGramAPIObject
	{

		[JsonProperty("friends")]
		public List<Friend> Friends { get; set;}

		[JsonProperty("friend")]
		public Friend Friend { get; set;}

		public FriendsObject () { }

	}
}