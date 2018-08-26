using UnityEngine;
using System.Collections;

/// <summary>
/// Checks for a value in the player dictionary.
/// If the dictionary indicates true for that key, turn this gameobject off
/// </summary>
public class NewFeatureIndicator : MonoBehaviour{

	private const string KEY_PREFIX = "NFI_";

	protected SaveModifier saveGame;
	protected GameManager gm;
	public string key { get; private set; }
	public bool selfInit;

	private bool registered;

	void Awake() {
		gm = GameManager.instance;
		saveGame = gm.save;
	}

	protected void Start() {
		if (selfInit)
			RegisterKey(key);
	}

	void OnEnable() {
		gm.save.OnHasViewedDictionaryUpdated += UpdateShouldEnable;
		if (registered)
			UpdateShouldEnable();
	}

	void OnDisable() {
		gm.save.OnHasViewedDictionaryUpdated -= UpdateShouldEnable;
	}

	public void RegisterKey(string k)
	{
		this.key = KEY_PREFIX + k;
		// print("Registered: " + key);
		if (!PlayerPrefs.HasKey(key)) {
			PlayerPrefs.SetInt(key, 0);
		}
		else
			UpdateShouldEnable();
		
		registered = true;
	}

	protected virtual void UpdateShouldEnable()
	{
		// print("Checking " + key);
		bool shouldEnable = PlayerPrefs.GetInt(key) == 0;
		// The object should only be enabled if the dictionary indicates that the player has not viewed the feature
		gameObject.SetActive(shouldEnable);
	}

	public void SetViewed() {
		if (key == "")
			return;
		print("Viewed: " + key);
		PlayerPrefs.SetInt(key, 1);
		gameObject.SetActive(false);
	}

}
