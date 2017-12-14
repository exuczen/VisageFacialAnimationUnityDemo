using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SimpleSQL;

namespace PictoryGramAPI.Data {
	public class UnlockedCharacter : PictoryGramAPIObject
	{
        [PrimaryKey] // SimpleSQL
		[JsonProperty("Id")]
		public long Id { get; set; }

		[JsonProperty("userId")]
		public string UserId { get; set; }

		[JsonProperty("characterId")]
		public string CharacterId { get; set; }
	}
}