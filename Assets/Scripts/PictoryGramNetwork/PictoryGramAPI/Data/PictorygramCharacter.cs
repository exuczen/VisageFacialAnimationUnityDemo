using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using SimpleSQL;

namespace PictoryGramAPI.Data {
	public class PictorygramCharacter
	{
        public const int STATUS_ACTIVE = 1;
        public const int STATUS_INACTIVE = 2;

        [PrimaryKey] // SimpleSQL
		[JsonProperty("characterId")]
		public string Id { get; set; }

		/// <summary>
		/// Displayed name
		/// </summary>
		/// <value>The name.</value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Category/group/folder of characters
		/// </summary>
		/// <value>The category identifier.</value>
		[JsonProperty("categoryId")]
		public long CategoryId { get; set; }

		[JsonProperty("createDate")]
		public DateTime Created { get; set; }

		[JsonProperty("modifiedDate")]
		public DateTime Modified { get; set; }

		/// <summary>
		/// If character is preloaded, this name should be set. For downloaded models we use id as a file name.
		/// </summary>
		/// <value>The name of the preloaded prefab.</value>
		[JsonProperty("preloadedPrefabName")]
		public string PreloadedPrefabName { get; set; }

		[JsonProperty("prefabUrl")]
		public string PrefabUrl { get; set; }

		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl { get; set; }

		[JsonProperty("revision")]
		public int Revision { get; set; }

		[JsonProperty("price")]
		public int Price { get; set; }

		/// <summary>
		/// availableForSale, campaign product, …  - enum
		/// </summary>
		/// <value>The status.</value>
		[JsonProperty("status")]
		public int Status { get; set; }

		/// <summary>
		/// Available for free for all users.
		/// </summary>
		/// <value><c>true</c> if this instance is default asset; otherwise, <c>false</c>.</value>
		[JsonProperty("isDefaultAsset")]
		public int IsDefaultAsset { get; set; }

		public PictorygramCharacter() { }
	}
}