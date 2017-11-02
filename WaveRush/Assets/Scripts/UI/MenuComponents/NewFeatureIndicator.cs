using UnityEngine;
using System.Collections;

/// <summary>
/// Checks for a value in the player dictionary.
/// If the dictionary indicates true for that key, turn this gameobject off
/// </summary>
public class NewFeatureIndicator : MonoBehaviour
{
	protected SaveGame saveGame;
	protected GameManager gm;
	public string key;
	public bool selfInit;

	void Awake()
	{
		gm = GameManager.instance;
		saveGame = gm.saveGame;
	}

	protected void Start()
	{
		if (selfInit)
			RegisterKey(key);
	}

	void OnEnable()
	{
		if (key == "")
			gm.OnHasViewedDictionaryUpdated += UpdateShouldEnable;
	}

	void OnDisable()
	{
		gm.OnHasViewedDictionaryUpdated -= UpdateShouldEnable;
	}

	public void RegisterKey(string key)
	{
		this.key = key;
		print("Registered: " + key);
		if (!saveGame.hasPlayerViewedDict.ContainsKey(key))
		{
			gm.SetHasPlayerViewedKey(key, false);
			gm.OnHasViewedDictionaryUpdated += UpdateShouldEnable;
		}
		else
			UpdateShouldEnable();
	}

	protected virtual void UpdateShouldEnable()
	{
		if (key == "")
			return;
		print("Checking " + key);
		bool shouldEnable = !saveGame.hasPlayerViewedDict[key];
		//print("Checking if " + key + " is new: " + shouldEnable);
		// The object should only be enabled if the dictionary indicates that the player has not viewed the feature
		gameObject.SetActive(shouldEnable);
	}

	public void SetViewed() 
	{
		gm.SetHasPlayerViewedKey(key, true);
	}

}
