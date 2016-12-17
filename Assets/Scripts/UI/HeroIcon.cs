using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroIcon : ScrollViewSnapContent {

	public string heroName;
	public bool unlocked { get; private set; }
	public int cost;

	private Animator anim;

	protected override void Awake()
	{
		base.Awake();
		anim = GetComponent<Animator> ();
	}

	public void SetLock(bool isUnlocked)
	{
		if (isUnlocked)
		{
			anim.CrossFade ("Unlocked", 0f);
			unlocked = true;
		}
		else
		{
			unlocked = false;
		}
	}

	public void Unlock()
	{
		anim.SetTrigger ("Animate");
		unlocked = true;
		GameManager.instance.saveGame.unlockedHeroes [index] = true;
		GameManager.instance.PrepareSaveFile ();
		print ("unlocked " + index);
		SaveLoad.Save ();
	}
}
