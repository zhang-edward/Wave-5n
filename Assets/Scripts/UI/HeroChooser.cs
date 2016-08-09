using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroChooser : MonoBehaviour
{
	public ScrollViewSnap heroIconsView;
	public HeroInfoPanel infoPanel;
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
		string hero = heroIconsView.SelectedContent.GetComponent<ScrollViewSnapContent> ().heroName;
		infoPanel.selectedHeroName = hero;
		infoPanel.DisplayHeroInfo ();
		scoreDisplay.DisplayScores (hero);
		GameManager.instance.selectedHero = hero;
	}
}

