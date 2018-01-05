using UnityEngine;
using System.Collections;

public class Mage_FireTrail : HeroPowerUp
{	
	public GameObject fireTrailEmitterPrefab;

	private MageHero mage;
	private Mage_Conflagration conflagrationPowerUp;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.mage = (MageHero)hero;
		conflagrationPowerUp = mage.GetComponentInChildren<Mage_Conflagration>();
		conflagrationPowerUp.onActivated += ActivateFireTrail;
	}

	public override void Deactivate ()
	{
		conflagrationPowerUp.onActivated -= ActivateFireTrail;
		base.Deactivate ();
	}

	private void ActivateFireTrail()
	{
		mage.OnMageShotFireball += CreateFireTrail;
	}

	private void CreateFireTrail(GameObject fireballObj)
	{
		GameObject o = Instantiate(fireTrailEmitterPrefab);
		o.GetComponent<EnemyDetectionZoneEmitter>().onDetectEnemy = BurnEnemy;
		o.transform.position = fireballObj.transform.position;
		o.transform.SetParent(fireballObj.transform);
		mage.OnMageShotFireball -= CreateFireTrail;
	}

	private void BurnEnemy(Enemy e)
	{
		print("burning enemy");
		GameObject burn = Instantiate(StatusEffectContainer.instance.GetStatus("Burn"));
		burn.GetComponent<BurnStatus>().damage = Mathf.CeilToInt(mage.damage * 0.1f);
		e.AddStatus(burn.gameObject);
	}
}

