using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageIcon : MonoBehaviour
{
	public delegate void OnClicked(GameObject obj);
	public OnClicked onClicked;

	public Image[] panels;
	public Sprite[] tierPanelSprites;
	public TMP_Text stageNameText;
	public TMP_Text stageLevelText;
	public Button highlightButton;      // the button that the user presses to expand the highlight menu
	public GameObject highlight;		// the shiny border around the window
	public GameObject highlightMenu;    // a menu with description and play button

	public StageData stage;
	public int seriesIndex;
	public int stageIndex;

	void Start() {
		highlightButton.onClick.AddListener(() => OnClick());
	}

	public void Init(StageData stage) {
		this.stage = stage;
		stageNameText.text = (stageIndex + 1) + " - " + stage.stageName;
		stageLevelText.text = "lv" + stage.difficultyLevel.ToString();

		Sprite panelSprite = tierPanelSprites[0];
		switch (stage.maxTier) {
			case HeroTier.tier2:
				panelSprite = tierPanelSprites[1];
				break;
			case HeroTier.tier3:
				panelSprite = tierPanelSprites[2];
				break;
		}
		foreach (Image img in panels) {
			img.sprite = panelSprite;
		}
	}

	private void OnClick() {
		if (onClicked != null)
			onClicked(this.gameObject);
	}

	public void ExpandHighlightMenu()
	{
		highlightMenu.SetActive(true);
	}

	public void CollapseHighlightMenu()
	{
		highlightMenu.SetActive(false);
	}
}
