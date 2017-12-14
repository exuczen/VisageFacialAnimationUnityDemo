using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PictoryGramAPI;
using System.Xml;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	public class Notification : PictoryGramAPIObject {

		/// <summary>
		/// The payload.
		/// </summary>
		[JsonProperty("payload")]
		public Payload Result { get; set; }

		/// <summary>
		/// The admin.
		/// </summary>
		[JsonProperty("author")]
		public Author Admin;

		/// <summary>
		/// The action.
		/// </summary>
		[JsonProperty("metadata")]
		public MetaData Action;

		public Notification() { }

		public Notification(string json) 
		{
			Result = new Payload();
			Result.Content = json;
		}

		public class Payload
		{
			[JsonProperty("content")]
			public string Content;
		}

		public struct Author
		{
			[JsonProperty("admin")]
			public int Admin { get; set; }
		}

		public struct MetaData
		{
			[JsonProperty("type")]
			public string Type { get; set; }
		}
	}
}