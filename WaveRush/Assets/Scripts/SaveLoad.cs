using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad
{
	public static void Save() {
		GameManager.instance.PrepareSaveFile ();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/save.gd");
		bf.Serialize(file, GameManager.instance.saveGame);
		file.Close();
		Debug.Log ("Saved Data");
		//GameManager.instance.DisplayMessage ("Saved Data");
	}

	public static void Load() {
		if(File.Exists(Application.persistentDataPath + "/save.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/save.gd", FileMode.Open);
			GameManager.instance.saveGame = (GameManager.SaveGame)bf.Deserialize(file);
			file.Close();

			GameManager.instance.LoadSaveFile ();
			//GameManager.instance.DisplayMessage ("Loaded Save Data");
			Debug.Log ("Loaded Saved Data");
		}
	}
}

