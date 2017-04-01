using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroChooser : MonoBehaviour
{
	public static string LOCKED = "LOCKED";

	public HeroIconsView heroIconsView;
	public HeroInfoPanelContainer infoPanel;
	public ScoreDisplay scoreDisplay;
	public Button playButton;

	void Awake()
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
		HeroIcon heroIcon = heroIconsView.SelectedContent.GetComponent<HeroIcon> ();
		HeroType heroName = HeroType.Null;
		if (heroIcon.unlocked)
		{
			heroName = heroIcon.heroName;
			GameManager.instance.SelectHero (heroName);	// select the hero in the GameManager
			playButton.interactable = true;
			// initialize info panel
			infoPanel.selectedHeroName = heroName;
			infoPanel.DisplayHeroInfo ();
		}
		else
		{
			infoPanel.DisplayLockedHero (this, heroIcon);
			playButton.interactable = false;
		}
		// display the scores for the selected hero (for locked heroes, display 0 for all)
		scoreDisplay.DisplayScores (heroName);
	}
}

