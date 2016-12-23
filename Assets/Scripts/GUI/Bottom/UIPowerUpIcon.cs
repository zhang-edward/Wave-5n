using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPowerUpIcon : MonoBehaviour
{
	private HeroPowerUp powerUp;
	public Image icon;

	public void Init(HeroPowerUp powerUp)
	{
		this.powerUp = powerUp;
		icon.sprite = powerUp.icon;
	}
}

