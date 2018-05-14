using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad {
	private const string SAVE_FILE_NAME = "save.gd";

	public static void Save(SaveGame sg) {
		GameManager.instance.PrepareSaveFile ();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/" + SAVE_FILE_NAME);
		bf.Serialize(file, sg);
		file.Close();
		Debug.Log ("Saved Data");
		//GameManager.instance.DisplayMessage ("Saved Data");
	}

	public static void Load(out SaveGame sg) {
		if (File.Exists(Application.persistentDataPath + "/" + SAVE_FILE_NAME)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + SAVE_FILE_NAME, FileMode.Open);
			sg = (SaveGame)bf.Deserialize(file);
			file.Close();

			GameManager.instance.LoadSaveFile();
			//GameManager.instance.DisplayMessage ("Loaded Save Data");
			Debug.Log("Loaded Saved Data");
		}
		else {
			Debug.LogWarning("No save file called " + SAVE_FILE_NAME + " detected.\n" +
							 "Check your " + Application.persistentDataPath + "/" + SAVE_FILE_NAME);
			sg = new SaveGame();
		}
	}
}

