using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIHeart : MonoBehaviour
{
	public bool empty { get; private set; }

	public SimpleAnimationPlayerImage anim;
	public Image image;
	public Sprite emptySprite;
	public Sprite fullSprite;

	public void SetEmpty()
	{
		anim.StopAllCoroutines ();
		empty = true;
		image.sprite = emptySprite;
	}

	public void SetFull()
	{
		anim.Play ();
		empty = false;
		image.sprite = fullSprite;
	}
}