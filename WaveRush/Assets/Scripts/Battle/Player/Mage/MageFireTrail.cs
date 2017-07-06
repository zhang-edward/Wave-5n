using UnityEngine;
using System.Collections;

public class MageFireTrail : HeroPowerUp
{
	public GameObject fireTrailEmitterPrefab;
	public GameObject fireTrailReadyIndicator;

	private MageHero mage;
	private float createFireTrailChance = 0.3f;
	private bool fireTrailReady = false;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.mage = (MageHero)hero;
		mage.OnMageShotFireball += CreateFireTrail;
		fireTrailReady = false;
		fireTrailReadyIndicator.SetActive (false);
	}

	public override void Deactivate ()
	{
		mage.OnMageShotFireball -= CreateFireTrail;
		base.Deactivate ();
	}

	public override void Stack ()
	{
		createFireTrailChance += 0.1f;
		base.Stack ();
	}

	private void CreateFireTrail(GameObject fireballObj)
	{
		if (fireTrailReady)
		{
			fireTrailReady = false;
			fireTrailReadyIndicator.SetActive (false);

			GameObject o = Instantiate (fireTrailEmitterPrefab);
			o.GetComponent<FireTrailEmitter>().damage = Mathf.RoundToInt(mage.damage * 0.5f);
			o.transform.position = fireballObj.transform.position;
			o.transform.SetParent (fireballObj.transform);
			percentActivated = 0f;
		}
		else if (Random.value < createFireTrailChance)
		{
			fireTrailReady = true;
			fireTrailReadyIndicator.SetActive (true);
			percentActivated = 1f;
		}
	}
}

