using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Unused
{
	/*public class HeroIcon : ScrollViewSnapContent
	{

		public HeroType heroName;
		public bool unlocked { get; private set; }
		public int cost;

		private Animator anim;

		public void Init(bool isUnlocked)
		{
			anim = GetComponent<Animator>();
			if (isUnlocked)
			{
				anim.CrossFade("Unlocked", 0f);
				unlocked = true;
			}
			else
			{
				unlocked = false;
			}
		}

		public void Unlock()
		{
			anim.SetTrigger("Animate");
			unlocked = true;
			GameManager.instance.saveGame.heroData[index].unlocked = true;
			GameManager.instance.PrepareSaveFile();
			print("unlocked " + index);
			SaveLoad.Save();
		}
	}*/
}