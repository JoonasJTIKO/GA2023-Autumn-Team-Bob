using System;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace TeamBobFPS.Save
{
	public class SaveSystem
	{
		public static event Action OnLoadingComplete;
		private ISaveReader reader;
		private ISaveWriter writer;

		public string SaveFolder
		{
			get
			{
				string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				return Path.Combine(documents, "TeamBobFPS", "Save");
			}
		}

		public string QuickSaveName
		{
			get { return "quickSave"; }
		}

        public string AutoSaveName
        {
            get { return "autoSave"; }
        }

		public string ConfigSaveName
        {
            get { return "config"; }
        }

        public string FileExtension
		{
			get { return ".save"; }
		}

		public SaveSystem(ISaveReader reader, ISaveWriter writer)
		{
			this.reader = reader;
			this.writer = writer;
		}

		public void QuickSave()
		{
			Save(Path.Combine(SaveFolder, QuickSaveName + FileExtension));
		}

        public void AutoSave()
        {
            Save(Path.Combine(SaveFolder, AutoSaveName + FileExtension));
        }

		public void ConfigSave()
        {
            SaveConfig(Path.Combine(SaveFolder, ConfigSaveName + FileExtension));
        }

        public void Save(string saveFilePath)
		{
			Debug.Log("Save");

			// TODO: Only QuickSave is implement. Implement the rest.
			if (!writer.PrepareWrite(saveFilePath))
			{
				// Something went wrong when preparing for save.
				Debug.LogError("Something went wrong while saving the game!");
				return;
			}

			// Finds references to all ISaveables in the scene.
			List<ISaveable> saveables =
				UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<ISaveable>().ToList();

			// The number of saveables in the scene
			writer.WriteInt(saveables.Count);

			foreach (var saveable in saveables)
			{
				saveable.Save(writer);
			}

			writer.FinalizeWrite();
		}

        public void SaveConfig(string saveFilePath)
        {
            Debug.Log("Save Config");

            if (!writer.PrepareWrite(saveFilePath))
            {
                // Something went wrong when preparing for save.
                Debug.LogError("Something went wrong while saving the config!");
                return;
            }

            // Finds references to all ISaveables in the scene.
            List<ISaveableConfig> saveables =
                UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<ISaveableConfig>().ToList();

            // The number of saveables in the scene
            writer.WriteInt(saveables.Count);

            foreach (var saveable in saveables)
            {
                saveable.Save(writer);
            }

            writer.FinalizeWrite();
        }

        public void QuickLoad()
		{
			Load(Path.Combine(SaveFolder, QuickSaveName + FileExtension));
		}

		public void AutoLoad()
		{
            Load(Path.Combine(SaveFolder, AutoSaveName + FileExtension));
        }

		public void ConfigLoad()
		{
            LoadConfig(Path.Combine(SaveFolder, ConfigSaveName + FileExtension));
        }

		public void Load(string saveFilePath)
		{
			Debug.Log("Load");

			if (!reader.PrepareRead(saveFilePath))
			{
				Debug.LogError("Something went wrong while loading save file" + saveFilePath);
				return;
			}

			List<ISaveable> saveables =
				UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<ISaveable>().ToList();

			// TODO: Instantiate new objects if there are not enough of them in the scene currently.

			// Number of saveables we have in the save file
			int savedCount = reader.ReadInt();
			for (int i = 0; i < savedCount; ++i)
			{
				SaveObjectType objectType = (SaveObjectType)reader.ReadInt();
				ISaveable saveable = saveables.FirstOrDefault(item => item.SaveType == objectType);
				if (saveable != null)
				{
					// Saveable is present in a scene, let's load its data.
					saveable.Load(reader);
					saveables.Remove(saveable);
				}
				else
				{
                    Debug.Log("Null saveable in the load list");
					Debug.Log(saveable);
                    // The saveable is not present in a scene. TODO: Load its prefab and instantiate it.
                }
			}
            reader.FinalizeRead();
			OnLoadingComplete?.Invoke();
        }

        public void LoadConfig(string saveFilePath)
        {
            Debug.Log("Load Config");

            if (!reader.PrepareRead(saveFilePath))
            {
                Debug.Log("Something went wrong while loading config save file (probably no config saved yet) " + saveFilePath);
                return;
            }

            List<ISaveableConfig> saveables =
                UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<ISaveableConfig>().ToList();

            // TODO: Instantiate new objects if there are not enough of them in the scene currently.

            // Number of saveables we have in the save file
            int savedCount = reader.ReadInt();
            for (int i = 0; i < savedCount; ++i)
            {
                SaveObjectType objectType = (SaveObjectType)reader.ReadInt();
                ISaveableConfig saveable = saveables.FirstOrDefault(item => item.SaveType == objectType);
                if (saveable != null)
                {
                    // Saveable is present in a scene, let's load its data.
                    saveable.Load(reader);
                    saveables.Remove(saveable);
                }
                else
                {
					Debug.Log("Null saveable in the config load list");
					// The saveable is not present in a scene. TODO: Load its prefab and instantiate it.
                }
            }

            reader.FinalizeRead();
        }
    }
}