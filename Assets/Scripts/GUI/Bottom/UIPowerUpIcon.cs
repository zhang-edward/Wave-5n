using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPowerUpIcon : MonoBehaviour
{
	private HeroPowerUp powerUp;
	public Image icon;
	private Image iconFrame;
	private Slider slider;

	void Awake()
	{
		iconFrame = GetComponent<Image> ();
		slider = GetComponent<Slider> ();
	}

	public void Init(HeroPowerUp powerUp)
	{
		this.powerUp = powerUp;
		icon.sprite = powerUp.icon;
	}

	void Update()
	{
		slider.value = 1f - powerUp.percentActivated;
		if (powerUp.percentActivated < 1f)
		{
			iconFrame.color = new Color (1, 1, 1, 0.5f);
			icon.color = new Color (1, 1, 1, 0.5f);
		}
		else
		{
			iconFrame.color = Color.white;
			icon.color = Color.white;
		}
	}
}

