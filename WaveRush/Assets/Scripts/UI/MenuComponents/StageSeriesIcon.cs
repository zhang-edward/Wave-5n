using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageSeriesIcon : ScrollViewSnapContent
{
	public delegate void Clicked(StageSeriesData data);
	public event Clicked onClicked;

	public Image icon;
	private StageSeriesData data;

	public void Init(StageSeriesData data)
	{
		this.data = data;
		icon.sprite = data.icon;
	}

	protected override void OnClick()
	{
		if (onClicked != null)
			onClicked(data);
		
	}
}
