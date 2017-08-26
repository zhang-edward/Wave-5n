using UnityEngine;
using UnityEngine.UI;

public class HoldDownButton : MonoBehaviour
{
	public Image outline;
	public Image button;
	public Sprite defaultSprite, holdDownSprite;

	private bool buttonHeldDown;
	private bool locked;
	public bool maxed { get; private set; }

	public void Update()
	{
		if (locked)
			return;
		if (buttonHeldDown)
		{
			outline.fillAmount += Time.deltaTime;
			if (outline.fillAmount >= 1f)
				maxed = true;
		}
		else
		{
			outline.fillAmount -= Time.deltaTime * 2f;
			maxed = false;
		}
	}

	public void ButtonHeldDown()
	{
		if (locked)
			return;
		buttonHeldDown = true;
		button.sprite = holdDownSprite;
	}

	public void ButtonReleased()
	{
		if (locked)
			return;
		buttonHeldDown = false;
		button.sprite = defaultSprite;
	}

	public void SetLocked(bool locked)
	{
		this.locked = locked;
	}
}
