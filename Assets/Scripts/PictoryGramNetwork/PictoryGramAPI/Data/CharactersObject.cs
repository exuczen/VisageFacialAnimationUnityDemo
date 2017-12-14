using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PictoryGramAPI.Request;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {

/// <summary>
/// Characters object.
/// </summary>
public class CharactersObject : PictoryGramAPIObject {

		//	public string Messages;
		[JsonProperty("characters")]
		public List<PictorygramCharacter> Characters { get; set;}

		public CharactersObject () { }
	}
}