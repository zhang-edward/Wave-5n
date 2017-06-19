using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Unused
{
	public class HeroInfoPanel : MonoBehaviour
	{

		[System.Serializable]
		public class AbilityInfo
		{
			public Sprite sprite;
			public string infoText;
		}
		public AbilityInfo ability1;
		public AbilityInfo specialAbility;
		public AbilityInfo ability2;

		public string defaultInfoText;

		[Header("Default Prefab Properties")]
		public GameObject ability1Obj;
		public GameObject specialAbilityObj;
		public GameObject ability2Obj;
		public ScrollingText scrollingText;

		void Start()
		{
			ScrollingTextOption abilityInfo1 = ability1Obj.GetComponent<ScrollingTextOption>();
			ScrollingTextOption abilityInfo2 = ability2Obj.GetComponent<ScrollingTextOption>();
			ScrollingTextOption specialAbilityInfo = specialAbilityObj.GetComponent<ScrollingTextOption>();

			abilityInfo1.scrollingText = scrollingText;
			abilityInfo2.scrollingText = scrollingText;
			specialAbilityInfo.scrollingText = scrollingText;

			abilityInfo1.text = ability1.infoText;
			abilityInfo2.text = ability2.infoText;
			specialAbilityInfo.text = specialAbility.infoText;

			ability1Obj.GetComponentInChildren<Image>().sprite = ability1.sprite;
			ability2Obj.GetComponentInChildren<Image>().sprite = ability2.sprite;
			specialAbilityObj.GetComponentInChildren<Image>().sprite = specialAbility.sprite;

			scrollingText.defaultText = defaultInfoText;
			scrollingText.text = defaultInfoText;
		}
	}
}
