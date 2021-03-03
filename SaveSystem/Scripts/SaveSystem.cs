using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Globalization;

namespace OxUE
{
	public class SaveSystem : Singleton<SaveSystem>, IInitableSystem
	{
		public bool IsInited { get; set; }

		public string currentSaveFileName = "???";

		private Hashtable playerPrefsHashtable = new Hashtable();
		private bool _hashTableChanged = false;

		private bool hashTableChanged
		{
			get { return _hashTableChanged; }
			set
			{
				_hashTableChanged = value;
				if (value) SaveSystem.Instance.Flush();
			}
		}

		private string serializedOutput = "";
		private string serializedInput = "";
		private const string PARAMETERS_SEPERATOR = ";";
		private const string KEY_VALUE_SEPERATOR = ":";

		private static string[] seperators = new string[] {PARAMETERS_SEPERATOR, KEY_VALUE_SEPERATOR};

		//NOTE modify the iw3q part to an arbitrary string of length 4 for your project, as this is the encryption key
		private bool wasEncrypted = false;
		private bool securityModeEnabled = true;

		public string GetCurrentSaveFileName()
		{
			return Instance.currentSaveFileName;
		}

		public string GetCurrentSaveFileFullPath()
		{
			return new FileInfo(Helpers.GetProjectDirectory("Save") + "/" + GetCurrentSaveFileName() + ".save").FullName;
		}

		public new void Awake()
		{
			base.Awake();
			DebugCommands.Instance.AddCommand("data", InstanceCommand, "Run a command for data system", "<command>", true);
			//Init();
		}

		public void Init()
		{
			//TODO:REVERT
			Init(Helpers.CreateMD5( /*SteamClient.SteamId.ToString()*/"GIRLEXE"));
		}

		public void Init(string file)
		{
			hashTableChanged = false;
			serializedOutput = "";
			serializedInput = "";

			if (!string.IsNullOrEmpty(file))
				currentSaveFileName = file;
			Debug.Log("[SaveSystem] Init from " + currentSaveFileName);
			//load previous settings
			StreamReader fileReader = null;


			if (File.Exists(GetCurrentSaveFileFullPath()))
			{
				try
				{
					fileReader = new StreamReader(GetCurrentSaveFileFullPath());
					wasEncrypted = true;
					serializedInput = Security.Decrypt(fileReader.ReadToEnd());
					fileReader.Dispose();
				}
				catch (Exception)
				{
					Debug.LogWarning("[SaveSystem] Outdated savefile found");
					fileReader.Dispose();
					File.Delete(GetCurrentSaveFileFullPath());
				}
			}

			playerPrefsHashtable.Clear();

			if (!string.IsNullOrEmpty(serializedInput))
			{
				//In the old PlayerPrefs, a WriteLine was used to write to the file.
				if (serializedInput.Length > 0 && serializedInput[serializedInput.Length - 1] == '\n')
				{
					serializedInput = serializedInput.Substring(0, serializedInput.Length - 1);

					if (serializedInput.Length > 0 && serializedInput[serializedInput.Length - 1] == '\r')
					{
						serializedInput = serializedInput.Substring(0, serializedInput.Length - 1);
					}
				}

				playerPrefsHashtable = Deserialize(serializedInput);
			}

			IsInited = true;
		}

		public bool HasKey(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			return playerPrefsHashtable.ContainsKey(key);
		}

		public void SetString(string key, string value, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public void SetInt(string key, int value, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public void SetFloat(string key, float value, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public void SetBool(string key, bool value, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public void SetLong(string key, long value, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public string GetString(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}

			return null;
		}

		public string GetString(string key, string defaultValue, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public int GetInt(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int) playerPrefsHashtable[key];
			}

			return 0;
		}

		public int GetInt(string key, int defaultValue, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public long GetLong(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (long) playerPrefsHashtable[key];
			}

			return 0;
		}

		public long GetLong(string key, long defaultValue, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (long) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public float GetFloat(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float) playerPrefsHashtable[key];
			}

			return 0.0f;
		}

		public float GetFloat(string key, float defaultValue, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public bool GetBool(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool) playerPrefsHashtable[key];
			}

			return false;
		}

		public bool GetBool(string key, bool defaultValue, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public void DeleteKey(string key, Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			playerPrefsHashtable.Remove(key);
		}

		public void DeleteAll(Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			playerPrefsHashtable.Clear();
		}

		public void Flush()
		{
			if (!hashTableChanged) return;

			serializedOutput = Serialize();

			string output = (securityModeEnabled ? Security.Encrypt(serializedOutput) : serializedOutput);

			Helpers.CheckDirectory("Save");

			File.Create(GetCurrentSaveFileFullPath()).Dispose();
			File.WriteAllText(GetCurrentSaveFileFullPath(), output);

			serializedOutput = "";
			hashTableChanged = false;
		}

		private string Serialize(Hashtable playerPrefsHashtable = null)
		{
			if (playerPrefsHashtable == null)
				playerPrefsHashtable = this.playerPrefsHashtable;

			string newSerializedOutput = "";

			IDictionaryEnumerator myEnumerator = playerPrefsHashtable.GetEnumerator();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool firstString = true;
			while (myEnumerator.MoveNext())
			{
				//if(serializedOutput != "")
				if (!firstString)
				{
					sb.Append(" ");
					sb.Append(PARAMETERS_SEPERATOR);
					sb.Append(" ");
				}

				sb.Append(EscapeNonSeperators(myEnumerator.Key.ToString(), seperators));
				sb.Append(" ");
				sb.Append(KEY_VALUE_SEPERATOR);
				sb.Append(" ");
				sb.Append(EscapeNonSeperators(myEnumerator.Value.ToString(), seperators));
				sb.Append(" ");
				sb.Append(KEY_VALUE_SEPERATOR);
				sb.Append(" ");
				sb.Append(myEnumerator.Value.GetType());
				firstString = false;
			}

			newSerializedOutput = sb.ToString();
			return newSerializedOutput;

		}

		private Hashtable Deserialize(string serializedInput)
		{
			Hashtable newPrefs = new Hashtable() { };
			string[] parameters = serializedInput.Split(new string[] {" " + PARAMETERS_SEPERATOR + " "},
				StringSplitOptions.RemoveEmptyEntries);

			foreach (string parameter in parameters)
			{
				string[] parameterContent =
					parameter.Split(new string[] {" " + KEY_VALUE_SEPERATOR + " "}, StringSplitOptions.None);

				newPrefs.Add(DeEscapeNonSeperators(parameterContent[0], seperators),
					GetTypeValue(parameterContent[2], DeEscapeNonSeperators(parameterContent[1], seperators)));

				if (parameterContent.Length > 3)
				{
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length +
					                 " elements");
				}
			}

			return newPrefs;
		}

		public string EscapeNonSeperators(string inputToEscape, string[] seperators)
		{
			inputToEscape = inputToEscape.Replace("\\", "\\\\");

			for (int i = 0; i < seperators.Length; ++i)
			{
				inputToEscape = inputToEscape.Replace(seperators[i], "\\" + seperators[i]);
			}

			return inputToEscape;
		}

		public string DeEscapeNonSeperators(string inputToDeEscape, string[] seperators)
		{

			for (int i = 0; i < seperators.Length; ++i)
			{
				inputToDeEscape = inputToDeEscape.Replace("\\" + seperators[i], seperators[i]);
			}

			inputToDeEscape = inputToDeEscape.Replace("\\\\", "\\");

			return inputToDeEscape;
		}

		private object GetTypeValue(string typeName, string value)
		{
			switch (typeName)
			{
				case "System.String":
					return (object) value.ToString();
				case "System.Int32":
					return Convert.ToInt32(value);
				case "System.Boolean":
					return Convert.ToBoolean(value);
				case "System.Single":
					//float
					return Convert.ToSingle(value);
				case "System.Int64":
					//long
					return Convert.ToInt64(value);
				default:
					Debug.LogError("Unsupported type: " + typeName);
					break;
			}

			return null;
		}

		private void InstanceCommand(string[] args)
		{
			if (args.Length == 1)
				Debug.Log(
					"Instance\nWorking commands:\ninit or init <savepath>\npath\nsetint <key> <value>\ngetint <key> <value>\nsetfloat <key> <value>\ngetfloat <key> <value>\nsetstring <key> <value>\ngetstring <key> <value>\nsetbool <key> <value>\ngetbool <key> <value>\ndelete <key>\ndeleteall");
			else if (args.Length == 2)
			{
				if (args[1] == "init")
				{
					Init();
				}
			}
			else
			{
				if (args[1] == "init")
				{
					if (args.Length != 3)
						Debug.LogError("Bad parameters for init! Use \"init\" or \"init <savename>\"");
					else
					{
						Init(args[2]);
					}
				}

				if (args[1] == "setint")
				{
					if (args.Length != 4)
						Debug.LogError("Bad parameters for setint! Use <key> <value>");
					else
					{
						Instance.SetInt(args[2], int.Parse(args[3], NumberStyles.Integer));
						Instance.Flush();
					}
				}

				if (args[1] == "getint")
				{
					if (args.Length != 3)
						Debug.LogError("Bad parameters for getint! Use <key> <value>");
					else
					{
						Debug.Log("Result for " + args[2] + " : " + Instance.GetInt(args[2]));
					}
				}

				if (args[1] == "setstring")
				{
					if (args.Length != 4)
						Debug.LogError("Bad parameters for setstring! Use <key> <value>");
					else
					{
						Instance.SetString(args[2], args[3]);
						Instance.Flush();
					}
				}

				if (args[1] == "getstring")
				{
					if (args.Length != 3)
						Debug.LogError("Bad parameters for getstring! Use <key> <value>");
					else
					{
						Debug.Log("Result for " + args[2] + " : " + Instance.GetString(args[2]));
					}
				}

				if (args[1] == "setbool")
				{
					if (args.Length != 4)
						Debug.LogError("Bad parameters for setbool! Use <key> <value>");
					else
					{
						Instance.SetBool(args[2], bool.Parse(args[3]));
						Instance.Flush();
					}
				}

				if (args[1] == "getbool")
				{
					if (args.Length != 3)
						Debug.LogError("Bad parameters for getbool! Use <key> <value>");
					else
					{
						Debug.Log("Result for " + args[2] + " : " + Instance.GetBool(args[2]));
					}
				}

				if (args[1] == "setfloat")
				{
					if (args.Length != 4)
						Debug.LogError("Bad parameters for setbool! Use <key> <value>");
					else
					{
						Instance.SetFloat(args[2], float.Parse(args[3], NumberStyles.Float));
						Instance.Flush();
					}
				}

				if (args[1] == "getfloat")
				{
					if (args.Length != 3)
						Debug.LogError("Bad parameters for getbool! Use <key> <value>");
					else
					{
						Debug.Log("Result for " + args[2] + " : " + Instance.GetFloat(args[2]));
					}
				}

				if (args[1] == "delete")
				{
					if (args.Length != 2)
						Debug.LogError("Bad parameters for delete! Use <key>");
					else
					{
						Instance.DeleteKey(args[2]);
					}
				}

				if (args[1] == "deleteall")
				{
					Instance.DeleteAll();
				}

				if (args[1] == "path")
				{
					Debug.Log("Save Path: " + Instance.GetCurrentSaveFileFullPath());
				}
			}
		}

		private void OnDestroy()
		{
			Flush();
		}
	}
}