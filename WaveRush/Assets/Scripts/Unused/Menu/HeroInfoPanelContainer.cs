namespace Unused
{
	/*using UnityEngine;
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

		public HeroType selectedHeroName;               // the name of the selected hero
		public HeroInfoPrefab[] heroInfoPrefabs;
		public GameObject lockedInfoPrefab;
		private GameObject currentHeroInfoPrefab;

		public Text nameField;


		public HeroInfoPrefab GetSelectedHeroInfo()
		{
			foreach (HeroInfoPrefab infoPrefab in heroInfoPrefabs)
				if (infoPrefab.heroName.Equals(selectedHeroName))
					return infoPrefab;
			// if selected hero could not be found
			throw new UnityEngine.Assertions.AssertionException("HeroInfoPanel in " + this.gameObject +
				" could not find hero info prefab with name \"" + selectedHeroName + "\"!",
				"Something messed up!");
			//return null;
		}

		public void DisplayHeroInfo()
		{
			// display hero name in name field
			nameField.text = selectedHeroName.ToString().ToUpper();
			// get the current hero info panel and destroy it
			if (currentHeroInfoPrefab != null)
				Destroy(currentHeroInfoPrefab.gameObject);
			// place the new hero info prefab
			currentHeroInfoPrefab = Instantiate(GetSelectedHeroInfo().infoPanelPrefab);
			currentHeroInfoPrefab.transform.SetParent(this.transform, false);
		}

		public void DisplayLockedHero(HeroChooser heroChooser, HeroIcon heroIcon)
		{
			// get the current hero info panel and destroy it
			if (currentHeroInfoPrefab != null)
				Destroy(currentHeroInfoPrefab.gameObject);
			// display LOCKED in name field
			nameField.text = HeroChooser.LOCKED;
			// display the button to unlock the hero
			currentHeroInfoPrefab = Instantiate(lockedInfoPrefab);
			currentHeroInfoPrefab.transform.SetParent(this.transform, false);
			LockedInfoPanel lockedHero = currentHeroInfoPrefab.GetComponent<LockedInfoPanel>();
			lockedHero.Init(heroChooser, heroIcon);
		}
	}*/
}

