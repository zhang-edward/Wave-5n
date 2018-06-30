using UnityEngine;
using System.Collections;

public class Mage_FireTrail : HeroPowerUp
{	
	private const float BURN_DAMAGE_MULTIPLIER = 0.05f;  
	private const float BURN_DURATION = 20f;            
	private const int NUM_BURN_SPREADS = 2;

	public GameObject fireTrailEmitterPrefab;

	private PyroHero mage;
	private Mage_Conflagration conflagrationPowerUp;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.mage = (PyroHero)hero;
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

	private void BurnEnemy(EnemyDetectionZone zone, Enemy e)
	{
		GameObject burnObj = Instantiate(StatusEffectContainer.instance.GetStatus("Burn"));
		BurnStatus burn = burnObj.GetComponent<BurnStatus>();
		burn.damage = Mathf.CeilToInt(mage.damage * BURN_DAMAGE_MULTIPLIER);
		burn.duration = BURN_DURATION;
		burn.numSpreads = NUM_BURN_SPREADS;
		e.AddStatus(burnObj);
	}
}

