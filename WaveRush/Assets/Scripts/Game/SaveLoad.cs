using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public class SaveLoad {
	private const string SAVE_FILE_NAME = "/save.gd";
	private const string PAWN_WALLET_NAME = "/pw.gd";

	public static void Save(SaveGame sg) {
		GameManager.instance.PrepareSaveFile ();

		// BinaryFormatter bf = new BinaryFormatter();
		// FileStream file = File.Create (Application.persistentDataPath + "/" + SAVE_FILE_NAME);
		string filePath = Application.persistentDataPath + SAVE_FILE_NAME;
		using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write)) {
			using (var writer = new BsonWriter(fs))	{
				var serializer = new JsonSerializer();
				serializer.Serialize(writer, sg);
			}
		}
		// bf.Serialize(file, sg);
		// file.Close();
		Debug.Log ("Saved Data");
		//GameManager.instance.DisplayMessage ("Saved Data");
	}

	public static void Load(ref SaveGame sg) {
		if (File.Exists(Application.persistentDataPath + SAVE_FILE_NAME)) {

			string filePath = Application.persistentDataPath + SAVE_FILE_NAME;
			sg = new SaveGame();

			using (var fs = File.OpenRead(filePath)) {
				using (var reader = new BsonReader(fs)) {
					var serializer = new JsonSerializer();
					serializer.Populate(reader, sg);
				}
			}


			GameManager.instance.LoadSaveFile();
		}
		else {
			Debug.LogWarning("No save file called " + SAVE_FILE_NAME + " detected.\n" +
							 "Check your " + Application.persistentDataPath + "/" + SAVE_FILE_NAME);
			GameManager.instance.CreateNewSave();
		}
	}
}

