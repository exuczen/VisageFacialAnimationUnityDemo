using UnityEngine;
using System.Collections;
using PictoryGramAPI.Request;
using Newtonsoft.Json;

namespace PictoryGramAPI.Data {
	
	/// <summary>
	/// Wrapper class for getting response from PictoryGramAPI.
	/// </summary>
	public class Response<T> : PictoryGramAPIWebRequest where T : PictoryGramAPIObject, new() {
	
	/// <summary>
	/// Deserialized data.
	/// </summary>
	public T Data { set; get; }
	
	public virtual void SetData(string json)
	{
		Data = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
	}
}
}