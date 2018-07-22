using UnityEngine;
using System.Collections;

public class NinjaTripleStar : HeroPowerUp
{
	private NinjaHero ninja;

	private const float CHARGE_THRESHOLD = 30f;
	private const int NUM_THROWS = 3;

	public float charge;
	public int numThrows;

	public override void Activate (PlayerHero hero)
	{
		base.Activate (hero);
		ninja = (NinjaHero)hero;
		ninja.player.OnEnemyDamaged += ChargeTripleStar;
		percentActivated = 0;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		ninja.player.OnEnemyDamaged -= ChargeTripleStar;
	}

	public void ChargeTripleStar(float amt)
	{
		charge += amt;
		percentActivated = charge / CHARGE_THRESHOLD;
		if (charge >= CHARGE_THRESHOLD)
		{
			percentActivated = 1;
			ninja.onTap += TripleStar;
			//ninja.onTap -= ninja.ShootNinjaStar;
			ninja.player.OnEnemyDamaged -= ChargeTripleStar;
			numThrows = NUM_THROWS;
		}
	}

	public void TripleStar(Vector3 dir)
	{
		// if cooldown has not finished
		if (!ninja.CheckIfCooledDownNotify (1))
			return;
		ninja.ResetCooldownTimer (1);

		numThrows--;
		if (numThrows <= 0)
		{
			// reset ability
			charge = 0;
			percentActivated = 0;
			ninja.onTap -= TripleStar;
			//ninja.onTap += ninja.ShootNinjaStar;
			ninja.player.OnEnemyDamaged += ChargeTripleStar;
		}

		StartCoroutine (TripleStarRoutine (dir));
	}

	private IEnumerator TripleStarRoutine(Vector3 dir)
	{
		// Sound
		SoundManager.instance.RandomizeSFX (ninja.shootSound);
		// Animation
		ninja.anim.Play ("Throw");

		for (int i = 0; i < 3; i ++)
		{
			// Player properties
			GameObject o = ninja.InitNinjaStar (dir.normalized);
			if (ninja.activatedSpecialAbility)
			//	ninja.ShootNinjaStarFanPattern ();
			// set direction
			ninja.body.Move (dir.normalized);
			ninja.body.rb2d.velocity = Vector2.zero;

			yield return new WaitForSeconds (0.1f);
		}

		ninja.body.moveSpeed = ninja.player.DEFAULT_SPEED;
		ninja.anim.Play("Throw");
	}
}

