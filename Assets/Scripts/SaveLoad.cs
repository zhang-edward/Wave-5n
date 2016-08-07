using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad
{
	public static void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/save.gd");
		bf.Serialize(file, GameManager.instance.scoreManager.highScores);
		file.Close();
		Debug.Log ("Saved Data");
	}

	public static void Load() {
		if(File.Exists(Application.persistentDataPath + "/save.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/save.gd", FileMode.Open);
			GameManager.instance.scoreManager.highScores = (Dictionary<string, ScoreManager.Score>)bf.Deserialize(file);
			file.Close();
			Debug.Log ("Loaded Saved Data");
		}
	}
}

