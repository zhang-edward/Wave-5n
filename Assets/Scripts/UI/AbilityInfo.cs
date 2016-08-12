using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityInfo : MonoBehaviour {

	public ScrollingText heroInfoText;
	public string infoText;

	public void UpdateHeroInfoText(bool isOn)
	{
		if (isOn)
			heroInfoText.UpdateText (infoText);
		else
			heroInfoText.SetToDefaultText ();
	}
}
