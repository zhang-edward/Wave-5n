using UnityEngine;

public class SettingsLoader : MonoBehaviour {

	public SettingsMenu settingsMenu;

	void Start() {
		settingsMenu.LoadSettings();
	}
}