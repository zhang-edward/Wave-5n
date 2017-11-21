using UnityEngine;

public class HeroesRescuedMenuButton : MonoBehaviour {

	public HeroesRescuedMenu hrm;
	public GameObject button;

	void Update()
	{
		if (hrm.AllIconsRevealed())
			button.SetActive(true);
	}

	void OnDisable() {
		button.SetActive(false);
	}
}
