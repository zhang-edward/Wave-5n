using UnityEngine;
using System.Collections;

public class HeroIconsView : ScrollViewSnap {

	protected override void InitContent ()
	{
		base.InitContent ();
		bool[] unlockedHeroes = GameManager.instance.saveGame.unlockedHeroes;
		for (int i = 0; i < content.Length; i ++)
		{
			HeroIcon heroIcon = content [i].GetComponent<HeroIcon> ();
			heroIcon.SetLock(unlockedHeroes[i]);
		}
	}
}
