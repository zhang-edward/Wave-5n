using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	GameManager gm;
	public PawnFusionMenu pawnFusionMenu;
	public PawnRetireMenu pawnRetireMenu;

	void Start()
	{
		gm = GameManager.instance;
		// initialize views on start to improve performance later
		pawnFusionMenu.Init();
		pawnRetireMenu.Init();
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
