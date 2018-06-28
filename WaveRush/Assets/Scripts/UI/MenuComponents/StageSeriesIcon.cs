using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public class StageSeriesIcon : MonoBehaviour {

	public delegate void Clicked(StageSeriesIcon icon);
	public event Clicked onClicked;

	[HideInInspector] public Button clickable;

	public Image icon;
	public TMP_Text seriesNameText;
	private StageSeriesData data;

	void Awake() {
		clickable = GetComponent<Button>();
	}

	public void Init(StageSeriesData data)
	{
		this.data = data;
		icon.sprite = data.icon;
		seriesNameText.text = data.seriesName;
		clickable.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		if (onClicked != null)
			onClicked(this);
	}

	public StageSeriesData GetData(){
		return data;
	}
}
