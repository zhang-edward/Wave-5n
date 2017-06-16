using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpIcon : MonoBehaviour
{
	public Image icon;
	public GameObject[] stacksIcons;

	private HeroPowerUp powerUp;
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
		icon.sprite = powerUp.data.icon;
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

		if (powerUp.stacks > 0)
		{
			UnityEngine.Assertions.Assert.IsTrue (powerUp.stacks <= 4);
			for (int i = 0; i < powerUp.stacks; i ++)
			{
				stacksIcons [i].SetActive (true);
			}
		}
	}
}

