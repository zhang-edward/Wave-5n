using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	GameManager gm;

	//[Header("Primary Menus")]
	//public PawnFusionMenu pawnFusionMenu;
	//public PawnRetireMenu pawnRetireMenu;
	
	[Header("Secondary Views")]
	public DialogueView dialogueView;

	public delegate void MainMenuEvent();
	public event MainMenuEvent OnGoToBattle;

	void Start()
	{
		gm = GameManager.instance;
		// initialize views on start to improve performance later
		//pawnFusionMenu.Init();
		//pawnRetireMenu.Init();
	}

	public void GoToBattle()
	{
		//if (gm.saveGame.pawnWallet.HasExtraPawns())
		//{
		//	gm.DisplayAlert("You have too many heroes! Try fusing or retiring them.");	
		//}
		//else if (!gm.saveGame.pawnWallet.HasPawns())
		//{
		//	gm.DisplayAlert("You don't have any heroes! Wait for the squire to recruit some or summon some with souls.");
		//}
		//else
		//{
			if (OnGoToBattle != null)
				OnGoToBattle();
		//}
	}
}
