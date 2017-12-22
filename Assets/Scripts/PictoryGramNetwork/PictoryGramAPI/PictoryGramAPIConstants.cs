using UnityEngine;
using System.Collections;

namespace PictoryGramAPI
{
	/// <summary>
	/// Constants class containing mostly strings for URLs.
	/// </summary>
	public class PictoryGramAPIConstants
	{
		#if UNITY_EDITOR
		private const bool USE_DEVELOP_SERVER = true;
		#else
		private const bool USE_DEVELOP_SERVER = false;
		#endif
		private const string DEVELOP_SERVER_URL = "http://pictorygramDev.pixzell.pl/json/";
		/// <summary>
		/// Server URL
		/// </summary>
		public const string PRODUCTION_SERVER_URL = USE_DEVELOP_SERVER ? DEVELOP_SERVER_URL : "http://ptale.mindpower.pl/json/";
        public const string RESET_USER_PASSWORD_URL = "http://pictorygramDev.pixzell.pl/json/users";
        public const int NUMBER_OF_DOWNLOADED_MESSAGES = 20;
		public const int NUMBER_OF_DOWNLOADED_FRIENDS = 50;
	}
}