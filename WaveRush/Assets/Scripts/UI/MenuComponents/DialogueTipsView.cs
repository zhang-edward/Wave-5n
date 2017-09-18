using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueTipsView : MonoBehaviour
{
	public Button button;
	public GameObject textPanel;
	public ScrollingText scrollingText;

	public string[] tips;
	private int currentTipIndex = -1;

	void Awake()
	{
		button.onClick.AddListener(DisplayNewTip);
	}

	private void DisplayNewTip()
	{
		int debugCounter = 0;
		int selectedTipIndex = Random.Range(0, tips.Length);
		while (selectedTipIndex == currentTipIndex && debugCounter < 1000)
		{
			selectedTipIndex = Random.Range(0, tips.Length);
			debugCounter++;
		}
		currentTipIndex = selectedTipIndex;
		textPanel.gameObject.SetActive(true);
		scrollingText.UpdateText(tips[currentTipIndex]);
	}
}
