using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PictoryGramAPI.Request;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	/// <summary>
	/// Class representing ScriptEndpoint structure.
	/// </summary>
	public class MessagesObject : PictoryGramAPIObject {


		//	public string Messages;
		[JsonProperty("messages")]
		public List<Message> Messages { get; set;}

		//	public string Users;
		[JsonProperty("users")]
		public List<Friend> Users { get; set;}


		public MessagesObject () { }

	}
}