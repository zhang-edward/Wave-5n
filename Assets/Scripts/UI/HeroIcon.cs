using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroIcon : ScrollViewSnapContent {
	
	public string heroName;
	public bool locked;

	public Image heroImage;
	public Image lockImage;

	public Animator anim;
}
