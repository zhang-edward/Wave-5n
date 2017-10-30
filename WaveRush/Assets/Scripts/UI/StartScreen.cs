using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{
	public void UserPressedScreen()
	{
		print("User pressed screen");
		if (PlayerPrefs.GetInt(SaveGame.TUTORIAL_COMPLETE_KEY) != 1)
		{
			GameManager.instance.GoToScene("Tutorial1", 0.1f);
		}
		else
		{
			GameManager.instance.GoToScene("MainMenu");
		}
	}
}
