using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PictoryGramAPI;
using PictoryGramAPI.Data;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	
	public class AdvertisementGroup : PictoryGramAPIObject 
	{
		[JsonProperty("advertisments")]
		public List<Advertisement> Advertisements { get; set;}

		public AdvertisementGroup () { }

	}
}