using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class PowerUpItem : MonoBehaviour
{
	private Image holderGraphic;
	public HolderGraphicTiersData data;
	public int cost;

	public int timesPurchased;
	public int purchaseLimit;
	public bool available { get; protected set; }

	public bool Selected {
		get {
			return GetComponent<Toggle> ().isOn;
		}
	}

	void Awake()
	{
		holderGraphic = GetComponent<Image>();
		available = true;
	}

	public virtual void Upgrade (Player player)
	{
		timesPurchased++;
		if (timesPurchased >= purchaseLimit)
			available = false;
	}

	protected void SetHolderGraphic(PowerUpTier tier)
	{
		Sprite graphic;
		switch(tier)
		{
			case PowerUpTier.tier1:
				graphic = data.tier1Graphic;
				break;
			case PowerUpTier.tier2:
				graphic = data.tier2Graphic;
				break;
			case PowerUpTier.tier3:
				graphic = data.tier3Graphic;
				break;
			default:
				graphic = data.tier1Graphic;
				break;
		}
		holderGraphic.sprite = graphic;
	}
}

