using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{
	void UserPressedScreen()
	{
		if (PlayerPrefs.GetInt(SaveGame.TUTORIAL_COMPLETE_KEY) != 1)
		{
			GameManager.instance.LoadScene("TutorialScene1");
		}
		else
		{
			GameManager.instance.LoadScene("MainMenu");
		}
	}
}
