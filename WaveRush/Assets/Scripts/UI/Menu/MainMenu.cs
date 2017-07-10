using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	GameManager gm;

	void Start()
	{
		gm = GameManager.instance;
	}

	public void GoToBattle()
	{
		if (gm.saveGame.HasExtraPawns())
		{
			Debug.LogWarning("Cannot enter; too many pawns!");	
		}
		else
		{
			GameManager.instance.GoToScene("HeroSelect");
		}
	}
}
