using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	public class UserCharactersObject : PictoryGramAPIObject {
		
		[JsonProperty("userCharacters")]
		public List<UnlockedCharacter> UserCharacters { get; set;}

		public UserCharactersObject () { }
	}
}