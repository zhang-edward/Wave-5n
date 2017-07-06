using UnityEngine;
using System.Collections;

public class MageFieryImpact : HeroPowerUp
{
	private MageHero mage;
	public GameObject mageFirePrefab;
	private float createFireChance = 0.3f;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.mage = (MageHero)hero;
		mage.OnMageTeleportIn += CreateFireZone;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		mage.OnMageTeleportIn -= CreateFireZone;
		createFireChance = 0.5f;
	}

	public override void Stack ()
	{
		base.Stack ();
		createFireChance += 0.1f;
	}

	private void CreateFireZone()
	{
		if (Random.value < createFireChance)
		{
			GameObject o = Instantiate(mageFirePrefab, transform.position, Quaternion.identity);
			o.GetComponent<MageFire>().damage = Mathf.RoundToInt(mage.damage * 0.5f);
		}
	}
}