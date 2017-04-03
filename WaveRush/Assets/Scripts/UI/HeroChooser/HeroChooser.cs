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
	public UpgradeScreen upgradeScreen;

	public Button playButton;
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
		if (heroIcon.unlocked)
		{
			heroName = heroIcon.heroName;
			GameManager.instance.SelectHero (heroName);	// select the hero in the GameManager
			playButton.interactable = true;
			// display text
			descriptionText.UpdateText(DataManager.GetDescriptionData(heroName).heroDescription);
			// animate score panel
		}
		else
		{
			playButton.interactable = false;
			descriptionText.UpdateText(LOCKED);
		}
		// display the scores for the selected hero (for locked heroes, display 0 for all)
		scoreDisplay.DisplayScores (heroName);
		scoreDisplay.GetComponent<Animator>().SetTrigger("Refresh");
	}

	private void ToUpgradeScreen()
	{
		HeroIcon heroIcon = heroIconsView.SelectedContent.GetComponent<HeroIcon>();
		upgradeScreen.Init(heroIcon.heroName);
	}
}

