using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{
	private bool pressed;

	public void UserPressedScreen()
	{
		if (!pressed) {
			// Navigate to the correct screen
			if (PlayerPrefs.GetInt(SaveGame.TUTORIAL_COMPLETE_KEY) != 1) {
				GameManager.instance.GoToScene(GameManager.SCENE_TUTORIAL, 0.1f);
			}
			else {
				GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
			}
			pressed = true;
		}
	}
}
