using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using PictoryGramAPI.Request;
using PictoryGramAPI.Data;
using System;

namespace PictoryGramAPI {
	/// <summary>
	/// Create PictoryGramAPI object. When using this constructor most functions will not work, because api key and instance name are usually required.
	/// </summary>
	public class PictoryGramAPIClient : SelfInstantiatingSingleton<PictoryGramAPIClient> {
	/// <summary>
	/// The name of the instance.
	/// </summary>
	public string InstanceName { get; private set; }

	/// <summary>
	/// Gets or sets the user key.
	/// </summary>
	/// <value>The user key.</value>
	public string UserKey { get; set; }

	/// <summary>
	/// Gets or sets the user key.
	/// </summary>
	/// <value>The user key.</value>
	public string UserID { get; set; }
	
	/// <summary>
	/// This flag checks if PictoryGramAPI client was initialized.
	/// </summary>
	private bool isInitialized;

	/// <summary>
	/// This method must be called before making any call to PictoryGramAPI.
	/// </summary>
	/// <param name="apiKey">API key.</param>
	/// <param name="instanceName">Instance name.</param>
	public PictoryGramAPIClient Init()
	{
		isInitialized = true;

		return this;
	}
	
	/// <summary>
	/// Returns builder object that makes it easy to configure a request in one line.
	/// </summary>
	public RequestBuilder Please()
	{	
		if(isInitialized == false)
		{
			Debug.LogError("PictoryGramAPI has not been initialized. Please call Init before making any requests.");
			return null;
		}
		return new RequestBuilder();
	}
	

	}
}