using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{
	public Animator logoAnimator;
	public AudioClip music;
	public AudioClip tappedScreenSound;
	private bool pressed;

	void Start() {
		SoundManager.instance.PlayMusicLoop(music);
	}

	public void UserPressedScreen()
	{
		if (!pressed) {
			pressed = true;
			SoundManager.instance.PlaySingle(tappedScreenSound);
			logoAnimator.CrossFade("Flash", 0);
			Invoke("GoToScene", 1.0f);
		}
	}

	private void GoToScene() {
		// Navigate to the correct screen
		if (PlayerPrefs.GetInt(SaveGame.TUTORIAL_COMPLETE_KEY) != 1) {
			GameManager.instance.GoToScene(GameManager.SCENE_TUTORIAL, 0.1f);
		}
		else {
			GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
		}
	}
}
