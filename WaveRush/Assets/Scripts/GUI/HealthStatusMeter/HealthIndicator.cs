using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthIndicator : MonoBehaviour
{
	public bool empty { get; private set; }

	public SimpleAnimationPlayerImage anim;
	public Image hardHealth;
	public Image softHealth;
	public Image hardHealthRaw;
	public Sprite emptySprite;
	public Sprite fullSprite;
	public Sprite halfSprite;
	public Sprite quarterSprite;
	public Sprite threeQuartersSprite;

	public void SetEmpty()
	{
		hardHealth.enabled = false;
		anim.StopAllCoroutines ();
		empty = true;
		softHealth.fillAmount = 0;
		hardHealthRaw.fillAmount = 0;
	}

	public void SetFull()
	{
		hardHealth.enabled = true;
		anim.Play ();
		empty = false;
		hardHealth.sprite = fullSprite;
		softHealth.fillAmount = 1;
		hardHealthRaw.fillAmount = 1;
	}

	public void SetQuarters(int numQuarters)
	{
		hardHealth.enabled = true;
		anim.StopAllCoroutines ();
		empty = true;
		hardHealth.sprite = halfSprite;
		switch (numQuarters)
		{
		case 0:
			SetEmpty();
			break;
		case 1:
			hardHealth.sprite = quarterSprite;
			break;
		case 2:
			hardHealth.sprite = halfSprite;
			break;
		case 3:
			hardHealth.sprite = threeQuartersSprite;
			break;
		default:
			Debug.LogError ("Unexpected fraction");
			break;
		}
	}

	public void SetSoftHealth(float amt) {
		softHealth.fillAmount = amt;
	}

	public void SetHardHealth(float amt) {
		hardHealthRaw.fillAmount = amt;
	}
}