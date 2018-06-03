using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageIcon : MonoBehaviour
{
	public delegate void OnClicked(GameObject obj);
	public OnClicked onClicked;

	public TMP_Text stageNameText;
	public TMP_Text stageLevelText;
	public Button highlightButton;      // the button that the user presses to expand the highlight menu
	public GameObject highlight;		// the shiny border around the window
	public GameObject highlightMenu;    // a menu with description and play button

	public int seriesIndex;
	public int stageIndex;

	void Start()
	{
		highlightButton.onClick.AddListener(() => OnClick());
	}

	public void Init(StageData stage)
	{
		stageNameText.text = stage.stageName;
		stageLevelText.text = "lv" + (Mathf.CeilToInt(stage.levelRaw * (2f/5f))).ToString();
	}

	private void OnClick()
	{
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
