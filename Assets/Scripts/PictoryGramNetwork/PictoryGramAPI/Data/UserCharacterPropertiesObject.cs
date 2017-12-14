using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PictoryGramAPI.Request;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {

/// <summary>
/// Characters object.
/// </summary>
public class UserCharacterPropertiesObject : PictoryGramAPIObject {

		//	public string userCharacterProperties;
		[JsonProperty("userCharacterProperties")]
		public List<UserCharacterProperty> userCharacterProperties { get; set;}

		public UserCharacterPropertiesObject () { }
	}
}