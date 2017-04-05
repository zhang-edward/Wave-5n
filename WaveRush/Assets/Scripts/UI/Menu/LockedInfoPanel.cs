using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LockedInfoPanel : MonoBehaviour {

	public Text costText;
	private HeroIcon hero;
	private HeroChooser chooser;
	public Button unlockButton;

	public void Init(HeroChooser chooser, HeroIcon hero)
	{
		this.chooser = chooser;
		this.hero = hero;
		costText.text = hero.cost.ToString();
		unlockButton.onClick.AddListener (UnlockHero);
	}

	private void UnlockHero()
	{
		Wallet wallet = GameManager.instance.wallet;
		if (wallet.TrySpend (hero.cost))
		{
			hero.Unlock ();
			chooser.UpdateHeroInfoPanel ();
		}
	}
}
