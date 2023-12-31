using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System;

namespace VNFramework.Core
{
    public class VNGameSave
    {
        public static GameSave[] ParseGameSaveText(string gameSaveText)
        {
            string pattern = @"<\|\s*(\[.*?\])\s*\|>";
            MatchCollection matches = Regex.Matches(gameSaveText, pattern, RegexOptions.Singleline);

            GameSave[] curGameSaves = matches
                .Select(match => ParseGameSaveItem(match.Groups[1].Value))
                .ToArray();

            return curGameSaves;
        }

        private static GameSave ParseGameSaveItem(string blockContent)
        {
            blockContent = blockContent.Trim();
            string pattern = @"\[\s*(save_index|save_date|mermaid_node|resume_pic|resume_text)\s*:\s*(.*?)\s*\]";
            MatchCollection matches = Regex.Matches(blockContent, pattern, RegexOptions.Singleline);

            var gameSave = new GameSave();

            foreach (Match match in matches.Cast<Match>())
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                switch (key)
                {
                    case "save_index": gameSave.SaveIndex = Convert.ToInt32(value.Trim()); break;
                    case "save_date": gameSave.SaveDate = value.Trim(); break;
                    case "mermaid_node": gameSave.MermaidNode = value.Trim(); break;
                    case "script_index": gameSave.VNScriptIndex = Convert.ToInt32(value.Trim()); break;
                    case "resume_pic": gameSave.ResumePic = value.Trim(); break;
                    case "resume_text": gameSave.ResumeText = value.Trim(); break;
                }
            }

            return gameSave;
        }

        public static string GameSavesToText(GameSave[] gameSaves)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < gameSaves.Length; i++)
            {
                if (gameSaves[i] != null && !string.IsNullOrWhiteSpace(gameSaves[i].SaveDate))
                {
                    sb.Append(gameSaves[i].ToString());
                }
            }

            return sb.ToString();
        }
    }
}