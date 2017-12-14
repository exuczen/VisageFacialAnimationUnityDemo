using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System;
using PictoryGramAPI.Data;
using Newtonsoft.Json;


namespace PictoryGramAPI {
/// <summary>
/// Class representing basic data structure for all DataObjects returned from PictoryGramAPI. Every class must override it, otherwise it won't deserialize properly.
/// </summary>
	public class PictoryGramAPIObject {

		public const string FIELD_STATUS = "status";
		public const string FIELD_ERROR = "error";
		public const string FIELD_METHOD = "method";
		public const string FIELD_TIMESTAMP = "timestamp";
		public const string FIELD_IP = "ip";

		/// <summary>
		/// The status of answer from PictoryGramAPI. (OK or ERROR)
		/// </summary>
		[JsonProperty(FIELD_STATUS, NullValueHandling = NullValueHandling.Ignore)]
		public string Status;
	
		/// <summary>
		/// The method of PictoryGramAPI request.
		/// </summary>
		[JsonProperty(FIELD_METHOD, NullValueHandling = NullValueHandling.Ignore)]
		public string Method;

		/// <summary>
		/// The Error message, if occured. Status value is ERROR
		/// </summary>
		[JsonProperty(FIELD_ERROR, NullValueHandling = NullValueHandling.Ignore)]
		public string Error;

		/// <summary>
		/// Timestamp of request
		/// </summary>
		[JsonProperty(FIELD_TIMESTAMP, NullValueHandling = NullValueHandling.Ignore)]
		public long Timestamp;

		/// <summary>
		/// Timestamp of request
		/// </summary>
		[JsonProperty(FIELD_IP, NullValueHandling = NullValueHandling.Ignore)]
		public string Ip;

		public PictoryGramAPIObject() { }
	}
}