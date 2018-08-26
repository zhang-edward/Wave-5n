using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SaveLoad {
	private const string SAVE_FILE_NAME = "/save.gd";
	private const string PAWN_WALLET_NAME = "/pw.gd";

	public static void Save(SaveGame sg) {
		Debug.Log("========== Saving ==========");
		GameManager.instance.PrepareSaveFile();
		string filePath = Application.persistentDataPath + SAVE_FILE_NAME;

		string saveJson = JsonConvert.SerializeObject(sg);
		Debug.Log("Serialized JSON: " + saveJson);
		
		SimpleAes aes = new SimpleAes();
		string encryptedSaveJson = aes.Encrypt(saveJson);
		Debug.Log("Encrypted JSON: " + encryptedSaveJson);

		File.WriteAllText(filePath, encryptedSaveJson);
		Debug.Log("========= Saving complete =========");
	}

	public static void Load(ref SaveGame sg) {
		Debug.Log("========== Loading ==========");
		if (File.Exists(Application.persistentDataPath + SAVE_FILE_NAME)) {

			string filePath = Application.persistentDataPath + SAVE_FILE_NAME;
			
			string encryptedSaveJson = File.ReadAllText(filePath);
			Debug.Log("Encrypted JSON: " + encryptedSaveJson);

			SimpleAes aes = new SimpleAes();
			string saveJson = aes.Decrypt(encryptedSaveJson);
			Debug.Log("Decrypted JSON: " + saveJson);

			if (saveJson != null) {
				sg = JsonConvert.DeserializeObject<SaveGame>(saveJson);
			}
			else {
				GameManager.instance.CreateNewSave();
			}

			GameManager.instance.LoadSaveFile();
		}
		else {
			Debug.LogWarning("No save file called " + SAVE_FILE_NAME + " detected.\n" +
							 "Check your " + Application.persistentDataPath + "/" + SAVE_FILE_NAME);
			GameManager.instance.CreateNewSave();
		}
		Debug.Log("========= Loading complete =========");
	}
}

