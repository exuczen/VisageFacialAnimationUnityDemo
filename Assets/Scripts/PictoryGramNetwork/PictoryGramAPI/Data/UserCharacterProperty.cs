using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using SimpleSQL;

namespace PictoryGramAPI.Data {
	public class UserCharacterProperty
	{

//        [PrimaryKey] // SimpleSQL
//		[JsonProperty("UserCharacterPropertyId")]
//		public string Id { get; set; }

		/// <summary>
		/// Name of parameter
		/// </summary>
		/// <value>The name of paramete.</value>
		[JsonProperty("parameterName")]
		public string ParameterName { get; set; }

		/// <summary>
		/// Value of parameter
		/// </summary>
		/// <value>0 - 100</value>
		[JsonProperty("parameterValue")]
		public string ParameterValue { get; set; }

		/// <summary>
		/// User who customized character
		/// </summary>
		/// <value>The user identifier.</value>
		[JsonProperty("userId")]
		public string UserId { get; set; }

		/// <summary>
		/// Customized character
		/// </summary>
		/// <value>The character identifier.</value>
		[JsonProperty("characterId")]
		public string CharacterId { get; set; }


		/// <summary>
		/// Message id
		/// </summary>
		/// <value>The message identifier. If empty means that is current user's character properties</value>
		[JsonProperty("messageId")]
		public string MessageId { get; set; }

		public UserCharacterProperty() { }
	}
}