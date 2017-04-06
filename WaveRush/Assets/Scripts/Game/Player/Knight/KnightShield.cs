using UnityEngine;
using System.Collections;

public class KnightShield : HeroPowerUp
{
	private KnightHero knight;
	public bool shielded;
	public float chargeCapacity;

	[HideInInspector]
	public float chargePerHit = 1;
	private float charge;

	[Header("Animations")]
	public SimpleAnimation shieldBreakAnim;
	public SimpleAnimation shieldRegenAnim;
	public GameObject effect;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		knight.player.OnPlayerDamaged += AbsorbDamage;
		knight.player.OnEnemyDamaged += Charge;
		ActivateShield ();
	}

	private void ActivateShield()
	{
		shielded = true;
		effect.SetActive (true);
	}

	public void AbsorbDamage(int amt)
	{
		if (!shielded)
			return;

		knight.player.Heal (amt);
		Deactivate ();
		effect.GetComponent<IndicatorEffect> ().AnimateOut ();
		CameraControl.instance.StartShake (0.1f, 0.05f);
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		chargePerHit = 1;
		shielded = false;
	}

	public override void Stack ()
	{
		base.Stack ();
		chargePerHit *= 1.5f;
	}

	private void Charge(float amt)
	{
		if (shielded)
			return;
		charge += (int)amt * chargePerHit;
		percentActivated = (float)charge / chargeCapacity;
		if (charge >= chargeCapacity)
		{
			ActivateShield ();
			charge = 0;
		}
	}
}

