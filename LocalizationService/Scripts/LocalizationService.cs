using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using Steamworks;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
//using Steamworks;

namespace OxUE
{
    public class LocalizationService : Singleton<LocalizationService>, IInitableSystem
    {
        public string defaultLanguage = "english";

        public static string LocalizationFilePath = "Data/Localization";


        public Action OnChangeLocalization;

        private string _localization = "eng";
        private Dictionary<string, Dictionary<string, string>> localizationLibrary;

        public string Localization
        {
            get { return _localization; }
            set
            {
                _localization = value;
                OnChangeLocalization?.Invoke();
            }
        }

        public bool IsInited { get; set; }

        public void Init()
        {
            LoadLocalization();
            string lang = defaultLanguage;

            Localization = lang;
            Debug.Log("[LocalizationService] Localization: " + Localization);

            DebugCommands.Instance.AddCommand("ls", RunCommand, "Run a command for the Localization Servise", "<command>");
            IsInited = true;
        }

        private void RunCommand(string[] args)
        {
            if (args.Length == 1)
                Debug.Log("Localization Servise\nWorking commands:\nset <language>\nlist\nkeys");
            else if (args.Length < 2)
                Debug.LogError("Command not found!");
            else
            {
                if (args[1] == "set")
                {
                    if (args.Length != 3)
                        Debug.LogError("Bad parameters for set! Use <language>");
                    else
                        Localization = args[2];
                }

                if (args[1] == "keys")
                {
                    if (args.Length != 3)
                        Debug.LogError("Bad parameters for set! Use <language>");
                    else
                        PrintKeys(args[2]);
                }

                if (args[1] == "list")
                {
                    PrintLoadedLanguages();
                }
            }
        }

        private void PrintLoadedLanguages()
        {
            Debug.Log("[LocalizationService] All languages: ");
            foreach (var item in localizationLibrary)
            {
                Debug.Log(item.Key + " - Contains " + item.Value.Count + " key(s)");
            }
        }

        private void PrintKeys(string language)
        {
            Debug.Log("[LocalizationService] All keys for " + language + ": ");
            foreach (var item in localizationLibrary[language])
            {
                Debug.Log("#" + item.Key + " = " + item.Value);
            }
        }

        #region Localize Logic

        public void LoadLocalization(List<string> localizationDirsList = default, bool addBase = true)
        {
            //initialize
            if (localizationDirsList == null)
                localizationDirsList = new List<string>();
            localizationDirsList.Add(Helpers.GetProjectDirectory(LocalizationFilePath, true));
            localizationLibrary = LoadLocalizeFileHelper(localizationDirsList);
            OnChangeLocalization?.Invoke();
        }

        private static Dictionary<string, Dictionary<string, string>> ParseLocalizeFile(string[,] grid)
        {
            // language
            var result = new Dictionary<string, Dictionary<string, string>>(grid.GetUpperBound(0) - 1);
            for (int col = 1; col < grid.GetUpperBound(0); col++)
                result.Add(grid[col, 0], new Dictionary<string, string>(grid.GetUpperBound(1) - 1));

            for (int ln = 1; ln < grid.GetUpperBound(1); ln++)
            for (int col = 1; col < grid.GetUpperBound(0); col++)
            {
                if (!string.IsNullOrEmpty(grid[0, ln]) && !string.IsNullOrEmpty(grid[col, ln]))
                {
                    if (!result[grid[col, 0]].ContainsKey(grid[0, ln]))
                        result[grid[col, 0]].Add(grid[0, ln], grid[col, ln]);
                    else
                    {
                        Debug.LogWarning("[LocalizationService] Dublicate found: " + grid[0, ln]);
                        result[grid[col, 0]][grid[0, ln]] = grid[col, ln];
                    }
                }
            }

            return result;
        }

        public string GetTextByKey(string key)
        {
            return GetTextByKeyWithLocalize(key, _localization);
        }

        public string GetLocalizatedText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            string result = text;
            var regex = new Regex(@"(?<=#)\w+");
            var matches = regex.Matches(text);
            foreach (Match key in matches)
            {
                //Debug.Log("Found world:" + key.Value);
                if (!string.IsNullOrEmpty(key.Value) && result.Contains("#" + key.Value))
                {
                    result = result.Replace("#" + key.Value, GetTextByKey(key.Value));
                    //Debug.Log(result);
                }
            }

            regex = new Regex(@"(?<={*.})\w+");
            matches = regex.Matches(text);
            foreach (Match key in matches)
            {
                Debug.Log("Found world:" + key.Value);
                /*if (!string.IsNullOrEmpty(key.Value) && result.Contains("#" + key.Value))
                {
                    result = result.Replace("#" + key.Value, GetTextByKey(key.Value));
                    //Debug.Log(result);
                }*/
            }

            result = result.Replace("#id", Localization);
            return result;
        }

        public string GetTextByKeyWithLocalize(string key, string localize)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(localize)) return "[EMPTY]";

            Dictionary<string, string> dictionary;
            if (localizationLibrary.TryGetValue(localize, out dictionary))
            {
                string result;
                if (dictionary.TryGetValue(key, out result))
                {
                    result = result.Replace("\\n", "\n");
                    return result;
                }
            }

            return string.Format("#{0}", key);
        }

        #endregion Localize Logic

        #region Helpers

        public string[] GetLocalizations()
        {
            var result = new string[localizationLibrary.Count];
            var i = 0;
            foreach (var loc in localizationLibrary)
            {
                result[i] = loc.Key;
                i++;
            }

            return result;
        }

        public Dictionary<string, Dictionary<string, string>> LoadLocalizeFileHelper(List<string> localizationDirsList)
        {
            Dictionary<string, Dictionary<string, string>> res = new Dictionary<string, Dictionary<string, string>>() { };
            //UnityEngine.Object[] languagesFiles = Resources.LoadAll(LocalizationFilePath, typeof(TextAsset));
            foreach (var localizationDir in localizationDirsList)
            {
                string path = new DirectoryInfo(localizationDir).FullName;
                foreach (var languages in Directory.GetFiles(path).ToList().Where(x => new FileInfo(x).Extension.ToLower() == ".csv"))
                {
                    Debug.Log("[LocalizationService] Loading localization: " + languages);
                    if (languages == null) return null;
                    FileInfo info = new FileInfo(languages);
                    Dictionary<string, Dictionary<string, string>> fileRes = LoadLocalizationFromFile(info.FullName);
                    foreach (var file in fileRes)
                    {
                        if (!res.Keys.Contains(file.Key))
                            res.Add(file.Key, file.Value);
                        else
                        {
                            foreach (var v in file.Value)
                            {
                                if (!res[file.Key].Keys.Contains(v.Key))
                                    res[file.Key].Add(v.Key, v.Value);
                                else
                                {
                                    res[file.Key][v.Key] = v.Value;
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        public string FixLocalizationFile(string fileContent)
        {
            return fileContent;
        }

        public string ClearText(string text)
        {
            string result = text;

            var regex = new Regex(@"(?<={)\w+");
            var matches = regex.Matches(result);
            foreach (Match match in matches)
            {
                result = result.Replace("{" + match.Value + "}", "");
            }

            regex = new Regex(@"(?<=#)\w+");
            matches = regex.Matches(result);
            foreach (Match match in matches)
            {
                result = result.Replace("#" + match.Value, "");
            }

            result = result.Replace("|", " ");

            return result;
        }

        public Dictionary<string, Dictionary<string, string>> LoadLocalizationFromFile(string filePath)
        {
            Dictionary<string, Dictionary<string, string>> res = new Dictionary<string, Dictionary<string, string>>() { };

            ResourcesManager.Instance.LoadTextWithLocalizationFix(filePath, filePath, true);
            //Debug.Log((string)ResourcesManager.GetLoadedResource(filePath));
            var resultGrid = CSVReader.SplitCsvGrid((string) ResourcesManager.GetLoadedResource(filePath));
            Dictionary<string, Dictionary<string, string>> plf = ParseLocalizeFile(resultGrid);

            foreach (var pl in plf)
            {
                if (!res.Keys.Contains(pl.Key))
                    res.Add(pl.Key, pl.Value);
                else
                {
                    foreach (var v in pl.Value)
                    {
                        res[pl.Key].Add(v.Key, v.Value);
                    }
                }
            }

            return res;
        }

        #endregion Helpers
    }
}