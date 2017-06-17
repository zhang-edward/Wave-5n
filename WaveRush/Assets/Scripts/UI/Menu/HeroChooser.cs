using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroChooser : MonoBehaviour
{
	public static string LOCKED = "LOCKED";

	public HeroIconsView heroIconsView;
	private int selectedContentIndex = -1;
	public ScrollingText descriptionText;
	//public HeroInfoPanelContainer infoPanel;
	public ScoreDisplay scoreDisplay;
	//public UpgradeScreen upgradeScreen;

	public Button playButton;
	public Button unlockButton;
	public Button toUpgradeScreen;

	void Awake()
	{
		toUpgradeScreen.onClick.AddListener(ToUpgradeScreen);
	}

	void Start()
	{
		UpdateHeroInfoPanel ();
	}

	void OnEnable()
	{
		heroIconsView.OnEndDrag += UpdateHeroInfoPanel;
	}

	void OnDisabled()
	{
		heroIconsView.OnEndDrag -= UpdateHeroInfoPanel;
	}

	public void UpdateHeroInfoPanel()
	{
		// check to see if hero selection changed
		if (selectedContentIndex == heroIconsView.selectedContentIndex)
			return;
		selectedContentIndex = heroIconsView.selectedContentIndex;

		// update the description panels
		HeroIcon heroIcon = heroIconsView.SelectedContent.GetComponent<HeroIcon> ();
		HeroType heroName = HeroType.Null;
		playButton.gameObject.SetActive(heroIcon.unlocked);		// if hero unlocked, show play button; else, hide it
		unlockButton.gameObject.SetActive(!heroIcon.unlocked);	// if hero unlocked, hide unlock button; else, show it
		if (heroIcon.unlocked)
		{
			heroName = heroIcon.heroName;
			//GameManager.instance.SelectHero (heroName);	// select the hero in the GameManager
			playButton.interactable = true;
			// display text
			descriptionText.UpdateText(DataManager.GetDescriptionData(heroName).heroDescription);
			// animate score panel
		}
		else
		{
			descriptionText.UpdateText(LOCKED);
		}
		// display the scores for the selected hero (for locked heroes, display 0 for all)
		scoreDisplay.DisplayScores (heroName);
		scoreDisplay.GetComponent<Animator>().CrossFade("Refresh", 0);
	}

	private void ToUpgradeScreen()
	{
		HeroIcon heroIcon = heroIconsView.SelectedContent.GetComponent<HeroIcon>();
		//upgradeScreen.Init(heroIcon.heroName);
	}

	public void UnlockHero()
	{
		HeroIcon hero = heroIconsView.SelectedContent.GetComponent<HeroIcon>();
		Wallet wallet = GameManager.instance.wallet;
		if (wallet.TrySpend(hero.cost))
		{
			hero.Unlock();
			UpdateHeroInfoPanel();
		}
	}
}

