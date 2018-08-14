using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour {

	public Color onColor, offColor;
	
	[Header("Audio Settings")]
	public CycledButton musicSetting;
	public CycledButton sfxSetting;
	public TMP_Text musicSettingText, sfxSettingText;

	[Header("Save Game Control")]
	public CycledButton deleteSaveButton;
	public TMP_Text deleteSaveButtonText;

	void Start() {
		musicSetting.OnButtonPressed += MusicSettingButtonPressed;
		sfxSetting.OnButtonPressed	 += SFxSettingButtonPressed;
		deleteSaveButton.OnButtonPressed += DeleteSaveButtonPresssed;
	}

	private void MusicSettingButtonPressed(int index) {
		if (index == 0) {
			musicSetting.button.targetGraphic.color = onColor;
			musicSettingText.text = "On";
			SoundManager.instance.MuteMusic(false);
		}
		else {
			musicSetting.button.targetGraphic.color = offColor;
			musicSettingText.text = "Off";
			SoundManager.instance.MuteMusic(true);
		}
	}

	private void SFxSettingButtonPressed(int index) {
		if (index == 0) {
			sfxSetting.button.targetGraphic.color = onColor;
			sfxSettingText.text = "On";
			SoundManager.instance.MuteSFX(false);
		}
		else {
			sfxSetting.button.targetGraphic.color = offColor;
			sfxSettingText.text = "Off";
			SoundManager.instance.MuteSFX(true);
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
}