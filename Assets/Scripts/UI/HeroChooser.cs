using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroChooser : MonoBehaviour
{
	public HeroIconsView heroIconsView;
	public HeroInfoPanelContainer infoPanel;
	public ScoreDisplay scoreDisplay;

	void Awake()
	{
	}

	// Use this for initialization
	void Start ()
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

	private void UpdateHeroInfoPanel()
	{
		// get the hero name from the scrollViewSnapContent
		string hero = heroIconsView.SelectedContent.GetComponent<HeroIcon> ().heroName;
		// initialize info panel
		infoPanel.selectedHeroName = hero;
		infoPanel.DisplayHeroInfo ();
		// display the scores for the selected hero
		scoreDisplay.DisplayScores (hero);
		// select the hero in the GameManager
		GameManager.instance.selectedHero = hero;
	}
}

