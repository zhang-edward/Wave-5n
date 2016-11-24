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
	public HeroInfoPrefab lockedInfoPrefab;
	private GameObject currentHeroInfoPrefab;

	public Text nameField;


	public HeroInfoPrefab GetSelectedHeroInfo()
	{
		if (selectedHeroName.Equals ("LOCKED"))
			return lockedInfoPrefab;
		
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
		// display hero name in name field
		nameField.text = selectedHeroName.ToUpper();
		// get the current hero info panel and destroy it
		if (currentHeroInfoPrefab != null)
			Destroy (currentHeroInfoPrefab.gameObject);
		// place the new hero info prefab
		currentHeroInfoPrefab = Instantiate (GetSelectedHeroInfo ().infoPanelPrefab);
		currentHeroInfoPrefab.transform.SetParent (this.transform, false);
	}
}

