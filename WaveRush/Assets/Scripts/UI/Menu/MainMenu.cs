using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	GameManager gm;

	[Header("Primary Menus")]
	public PawnFusionMenu pawnFusionMenu;
	public PawnRetireMenu pawnRetireMenu;
	
	[Header("Secondary Views")]
	public DialogueView dialogueView;

	void Start()
	{
		gm = GameManager.instance;
		// initialize views on start to improve performance later
		pawnFusionMenu.Init();
		pawnRetireMenu.Init();
	}

	public void GoToBattle()
	{
		if (gm.saveGame.pawnWallet.HasExtraPawns())
		{
			gm.DisplayAlert("You have too many heroes! Try fusing or retiring them.");	
		}
		else
		{
			GameManager.instance.GoToScene("HeroSelect");
		}
	}
}
