using UnityEngine;
using System.Collections;

/// <summary>
/// Checks for a value in the player dictionary.
/// If the dictionary indicates true for that key, turn this gameobject off
/// </summary>
public class NewFeatureIndicator : MonoBehaviour
{
	private SaveGame saveGame;
	private GameManager gm;
	private string key;

	void Awake()
	{
		gm = GameManager.instance;
		saveGame = gm.saveGame;
	}

	void OnEnable()
	{
		gm.OnHasViewedDictionaryUpdated += UpdateShouldEnable;
	}

	void OnDisable()
	{
		gm.OnHasViewedDictionaryUpdated -= UpdateShouldEnable;
	}

	public void RegisterKey(string key)
	{
		this.key = key;
		print(key);
		if (!saveGame.hasPlayerViewedDict.ContainsKey(key))
			gm.SetHasPlayerViewedKey(key, false);
		else
			UpdateShouldEnable();
	}

	private void UpdateShouldEnable()
	{
		if (key == null)
			return;
		bool shouldEnable = !saveGame.hasPlayerViewedDict[key];
		print("Checking if " + key + " is new: " + shouldEnable);
		// The object should only be enabled if the dictionary indicates that the player has not viewed the feature
		gameObject.SetActive(shouldEnable);
	}
}
