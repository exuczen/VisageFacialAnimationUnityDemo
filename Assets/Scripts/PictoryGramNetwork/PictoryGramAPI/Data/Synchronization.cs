using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PictoryGramAPI.Data;
using Newtonsoft.Json;
using SimpleSQL;

namespace PictoryGramAPI.Data {
	/// <summary>
	/// All message's properties sent by a user.
	/// </summary>
	/// 
	public enum SynchronizationMethod
	{
		NEW_RECEIVED_MESSAGE = 0,
		NEW_SENT_MESSAGE = 1,
		OLD_RECEIVED_MESSAGE = 2,
		OLD_SENT_MESSAGE = 3,
		NEW_IMPORTED_FRIENDS = 4,
		OLD_IMPORTED_FRIENDS = 5,
		NEW_FRIENDS = 6,
		OLD_FRIENDS = 7,
		NOT_FRIENDS = 8,
	}

	public class Synchronization
	{

		[JsonProperty("Method")]
		public SynchronizationMethod Method { get; set; }

		[JsonProperty("TableId")]
		public string TableId { get; set; }

		[JsonProperty("Timestamp")]
		public long Timestamp { get; set; }

		public Synchronization ()
		{
		}
	}
}

