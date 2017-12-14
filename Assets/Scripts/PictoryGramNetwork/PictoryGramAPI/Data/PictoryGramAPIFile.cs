using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	public class PictoryGramAPIFile {	

		[JsonProperty("data")]
		public byte[] Data { get; set; }

		public PictoryGramAPIFile() { }

		public PictoryGramAPIFile(byte[] data) 
		{
			Data = data;
		}
	}
}