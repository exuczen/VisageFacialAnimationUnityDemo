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
    public class Message
    {
        public enum MessageStatusEnum
        {
            DELETED = 0,
            UNREAD = 1,
            READ = 2,
        }

        [PrimaryKey] // SimpleSQL
    	[JsonProperty("messageID")]
    	public string MessageID { get; set; }

    	[JsonProperty("senderId")]
    	public string SenderId { get; set; }

    	[JsonProperty("recipientId")]
    	public string RecipientId { get; set; }

    	[JsonProperty("subject")]
    	public string Subject { get; set; }

    	[JsonProperty("characterId")]
    	public string CharacterId { get; set; }

        [JsonProperty("animation")]
        public string Animation { get; set; }

        [Ignore] // SimpleSQL
    	[JsonProperty("recording")]
    	public PictoryGramAPIFile Recording { get; set; } // This variable is only ofr file upload. Should never be stored.

        [JsonProperty("soundUrl")]
        public string RecordingUrl { get; set; } // Url is available after message is uploaded.

		[JsonProperty("recordingIsCompressed")]
		public int RecordingIsCompressed { get; set; } //mp3 is compressed (1), wav is not (0)

		[JsonProperty("createDate")]
		public DateTime CreateDate { get; set; } //createDate in format YYYY-MM-DD HH:MM:SS

		[JsonProperty("groupID")]
    	public string GroupID { get; set; }

        [JsonProperty("status")]
        public MessageStatusEnum Status { get; set; }

		//	public string userCharacterProperties;
		[Ignore] // SimpleSQL
		[JsonProperty("characterProperties")]
		public List<UserCharacterProperty> UserCharacterProperties { get; set;}

    	public Message() { }

    	public Message(string messageID)
    	{
    		this.MessageID = messageID;
    	}

    	public Message(string senderId, string recipentId, string subject, string characterId, string animation, PictoryGramAPIFile recording)
    	{
    		SenderId = senderId;
    		RecipientId = recipentId;
    		Subject = subject;
    		CharacterId = characterId;
            Animation = animation;
    		Recording = recording;
    	}
    }
}
