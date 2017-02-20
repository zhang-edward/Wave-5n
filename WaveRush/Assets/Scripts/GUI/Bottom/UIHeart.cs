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
	public Sprite halfSprite;
	public Sprite quarterSprite;
	public Sprite threeQuartersSprite;

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

	public void SetQuarters(int numQuarters)
	{
		anim.StopAllCoroutines ();
		empty = true;
		image.sprite = halfSprite;
		switch (numQuarters)
		{
		case 1:
			image.sprite = quarterSprite;
			break;
		case 2:
			image.sprite = halfSprite;
			break;
		case 3:
			image.sprite = threeQuartersSprite;
			break;
		default:
			Debug.LogError ("Unexpected fraction");
			break;
		}
	}
}