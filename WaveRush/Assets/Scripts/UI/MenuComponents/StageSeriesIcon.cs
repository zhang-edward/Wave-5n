using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public class StageSeriesIcon : ScrollViewSnapContent {
	public delegate void Clicked(StageSeriesIcon icon);
	public event Clicked onClicked;

	public Image icon;
	public TMP_Text seriesNameText;
	public Button button;
	private StageSeriesData data;

	public void Init(StageSeriesData data)
	{
		this.data = data;
		icon.sprite = data.icon;
		seriesNameText.text = data.seriesName;
		button.onClick.AddListener(OnClick);
	}

	protected override void OnClick()
	{
		if (onClicked != null)
			onClicked(this);
	}

	public StageSeriesData GetData(){
		return data;
	}
}
