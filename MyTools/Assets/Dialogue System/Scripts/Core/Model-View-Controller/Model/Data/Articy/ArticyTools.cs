using System.IO;
using System.Text.RegularExpressions;

namespace PixelCrushers.DialogueSystem.Articy {
	
	/// <summary>
	/// This static utility class contains tools for working with Articy data.
	/// </summary>
	public static class ArticyTools {

        /// <summary>
        /// Checks the first few lines of a string in articy:draft XML format for a schema identifier.
        /// </summary>
        /// <returns>
        /// <c>true</c> if it contains the schema identifier.
        /// </returns>
        /// <param name='xmlFilename'>
        /// XML data to check.
        /// </param>
        /// <param name='schemaId'>
        /// Schema identifier to check for.
        /// </param>
        public static bool DataContainsSchemaId(string xmlData, string schemaId)
        {
            StringReader xmlStream = new StringReader(xmlData);
            if (xmlStream != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    string s = xmlStream.ReadLine();
                    if (s.Contains(schemaId)) return true;
                }
                //--- Not compatible with UWP10: xmlStream.Close();
            }
            return false;
        }

        private static string[] htmlTags = new string[] { "<html>", "<head>", "<style>", "#s0", "{text-align:left;}", "#s1", 
			"{font-size:11pt;}", "</style>", "</head>", "<body>", "<p id=\"s0\">", "<span id=\"s1\">",
			"</span>", "</p>", "</body>", "</html>" };
		
		/// <summary>
		/// Removes HTML tags from a string.
		/// </summary>
		/// <returns>
		/// The string without HTML.
		/// </returns>
		/// <param name='s'>
		/// The HTML-filled string.
		/// </param>
		public static string RemoveHtml(string s) {
			// This is a rather inefficient first pass, but it gets the job done.
			// On the roadmap: Replace with http://www.codeproject.com/Articles/298519/Fast-Token-Replacement-in-Csharp
			if (!string.IsNullOrEmpty(s)) {
				foreach (string htmlTag in htmlTags) {
					s = s.Replace(htmlTag, string.Empty);
				}
				s = s.Replace("&#39;", "'");
				s = s.Replace("&quot;", "\"");
				s = s.Replace("&amp;", "&");
				s = s.Trim();
			}
			return s;
		}
		
		public static bool IsQuestStateArticyPropertyName(string propertyName) {
			return string.Equals(propertyName, "State") ||
				Regex.Match(propertyName, @"^Entry_[0-9]+_State").Success;
		}

		public static string EnumValueToQuestState(int enumValue, string stringValue) {
            // In case enum is out of order, go by string value:
            if (string.Equals("unassigned", stringValue, System.StringComparison.OrdinalIgnoreCase)) return QuestLog.StateToString(QuestState.Unassigned);
            if (string.Equals("active", stringValue, System.StringComparison.OrdinalIgnoreCase)) return QuestLog.StateToString(QuestState.Active);
            if (string.Equals("success", stringValue, System.StringComparison.OrdinalIgnoreCase)) return QuestLog.StateToString(QuestState.Success);
            if (string.Equals("failure", stringValue, System.StringComparison.OrdinalIgnoreCase)) return QuestLog.StateToString(QuestState.Failure);
            if (string.Equals("abandoned", stringValue, System.StringComparison.OrdinalIgnoreCase)) return QuestLog.StateToString(QuestState.Abandoned);

            // Failing that, by enum value:
            switch (enumValue) {
			case 1: return QuestLog.StateToString(QuestState.Unassigned);
			case 2: return QuestLog.StateToString(QuestState.Active);
			case 3: return QuestLog.StateToString(QuestState.Success);
			case 4: return QuestLog.StateToString(QuestState.Failure);
			case 5: return QuestLog.StateToString(QuestState.Abandoned);
			default: return QuestLog.StateToString(QuestState.Unassigned);
			}
		}
		
	}
		 
}