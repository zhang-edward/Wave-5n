using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	GameManager gm;

	public Toggle bottomNavButton;

	[Header("Primary Menus")]
	public PawnShopMenu pawnShopMenu;
	//public PawnRetireMenu pawnRetireMenu;

	[Header("Secondary Views")]
	public PawnInfoPanel pawnInfoPanel;
	public DialogueView dialogueView;
	public GameObject heroUnlockedNotifyPanel;
	public HeroTypeIcon heroUnlockedNotifyPanelIcon;

	public delegate void MainMenuEvent();
	public event MainMenuEvent OnGoToBattle;

	void Start()
	{
		gm = GameManager.instance;
		pawnShopMenu.Init();
		bottomNavButton.onValueChanged.AddListener(OnNavigatedToMainMenu);
		OnNavigatedToMainMenu(true);
	}

	public void GoToBattle()
	{
		if (OnGoToBattle != null)
			OnGoToBattle();
	}

	private void OnNavigatedToMainMenu(bool isOn) {
		if (!isOn)
			return;
		StartCoroutine(NotifyHeroUnlockedRoutine());
	}

	private IEnumerator	NotifyHeroUnlockedRoutine() {
		bool[] heroJustUnlocked = gm.heroJustUnlocked;
		for (int i = 0; i < heroJustUnlocked.Length; i ++) {
			if (heroJustUnlocked[i]) {
				heroUnlockedNotifyPanelIcon.Init(Pawn.Index2Type(i), Pawn.Index2Tier(i));
				heroUnlockedNotifyPanel.gameObject.SetActive(true);
				gm.heroJustUnlocked[i] = false;
				yield return new WaitWhile(() => heroUnlockedNotifyPanel.gameObject.activeInHierarchy);
			}
		}
		print("Done checking heroes unlocked");
	}
}
