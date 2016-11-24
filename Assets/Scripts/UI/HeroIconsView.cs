using UnityEngine;
using System.Collections;

public class HeroIconsView : ScrollViewSnap {

	public bool[] heroLocked;

	protected override void InitContent ()
	{
		base.InitContent ();
		for (int i = 0; i < content.Length; i ++)
		{
			HeroIcon heroIcon = content [i].GetComponent<HeroIcon> ();
			heroIcon.locked = heroLocked[i];
		}
	}
}
