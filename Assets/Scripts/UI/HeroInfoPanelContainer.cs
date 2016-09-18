using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroInfoPanelContainer : MonoBehaviour
{
	[System.Serializable]
	public class HeroInfoPrefab 
	{
		public string heroName;
		public GameObject infoPanelPrefab;
	}

	public string selectedHeroName;
	public HeroInfoPrefab[] heroInfoPrefabs;
	private GameObject currentHeroInfoPrefab;

	public Text nameField;


	public HeroInfoPrefab GetSelectedHeroInfo()
	{
		foreach (HeroInfoPrefab infoPrefab in heroInfoPrefabs)
			if (infoPrefab.heroName.Equals (selectedHeroName))
				return infoPrefab;
		// if selected hero could not be found
		throw new UnityEngine.Assertions.AssertionException ("HeroInfoPanel in " + this.gameObject + 
			" could not find hero info prefab with name \"" + selectedHeroName + "\"!",
			"Something messed up!");
		//return null;
	}

	public void DisplayHeroInfo()
	{
		nameField.text = selectedHeroName.ToUpper();
		if (currentHeroInfoPrefab != null)
			Destroy (currentHeroInfoPrefab.gameObject);
		currentHeroInfoPrefab = Instantiate (GetSelectedHeroInfo ().infoPanelPrefab);
		currentHeroInfoPrefab.transform.SetParent (this.transform, false);
	}
}

