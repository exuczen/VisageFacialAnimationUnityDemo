using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	
	public class Advertisement  
	{
		/// <summary>
		/// Gets or sets the ad URL.
		/// </summary>
		/// <value>The ad UR.</value>
		[JsonProperty("ad_url")]
		public string AdURL { get; set; }
	}
}