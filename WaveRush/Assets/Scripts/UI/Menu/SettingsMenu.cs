using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour {

	public string MUSIC_KEY = "Setting_MusicMuted";
	public string SFX_KEY = "Setting_SFXMuted";

	public Color onColor, offColor;
	
	[Header("Audio Settings")]
	public CycledButton musicSetting;
	public CycledButton sfxSetting;
	public TMP_Text musicSettingText, sfxSettingText;

	[Header("Save Game Control")]
	public CycledButton deleteSaveButton;
	public TMP_Text deleteSaveButtonText;


	void Start() {
		gameObject.SetActive(false);
		// Set up events
		musicSetting.OnButtonPressed += MusicSettingButtonPressed;
		sfxSetting.OnButtonPressed	 += SFxSettingButtonPressed;
		deleteSaveButton.OnButtonPressed += DeleteSaveButtonPresssed;
	}

	private void MusicSettingButtonPressed(int index) {
		// If index == 0, music is on
		SetToggle(musicSetting, musicSettingText, index == 0);
		if (index != 0)
			print ("Muting music");
		SoundManager.instance.MuteMusic(index != 0);
		PlayerPrefs.SetInt(MUSIC_KEY, index);
	}

	private void SFxSettingButtonPressed(int index) {
		// If index == 0, sfx is on
		SetToggle(sfxSetting, sfxSettingText, index == 0);
		SoundManager.instance.MuteSFX(index != 0);
		PlayerPrefs.SetInt(SFX_KEY, index);
	}

	private void SetToggle(CycledButton cycledButton, TMP_Text text, bool on) {
		if (on) {
			cycledButton.button.targetGraphic.color = onColor;
			cycledButton.cycleIndex = 0;
			text.text = "On";
		}
		else {
			cycledButton.button.targetGraphic.color = offColor;
			cycledButton.cycleIndex = 1;
			text.text = "Off";
		}
	}

	private void DeleteSaveButtonPresssed(int index) {
		switch (index) {
			case 0:
				deleteSaveButtonText.text = "Delete Save Data";
				break;
			case 1:
				deleteSaveButtonText.text = "Are you sure?";
				break;
			case 2:
				deleteSaveButtonText.text = "This cannot be undone!";
				break;
			case 3:
				deleteSaveButtonText.text = "Tap again to confirm.";
				break;
			case 4:
				deleteSaveButtonText.text = "Deleted save file";
				GameManager.instance.DeleteSaveData();
				GameManager.instance.GoToScene(GameManager.SCENE_STARTSCREEN);
				break;
		}
	}

	public void LoadSettings() {
		int musicOn = PlayerPrefs.GetInt(MUSIC_KEY, 0);
		int sfxOn = PlayerPrefs.GetInt(SFX_KEY, 0);
		MusicSettingButtonPressed(musicOn);
		SFxSettingButtonPressed(sfxOn);
	}
}