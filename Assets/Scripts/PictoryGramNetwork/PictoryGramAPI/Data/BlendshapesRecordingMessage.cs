using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PictoryGramAPI.Data;
using Newtonsoft.Json;
using SimpleSQL;

namespace PictoryGramAPI.Data {
    /// <summary>
    /// All message's properties sent by a user.
    /// </summary>
    public class BlendshapesRecordingMessage : Message
    {
		[Ignore] // SimpleSQL
		[JsonProperty("blendshapesRecording")]
		public PictoryGramAPIFile BlendshapesRecording { get; set; } // This variable is only ofr file upload. Should never be stored.
	}
}
