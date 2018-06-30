using UnityEngine;
using System.Collections;

public class Knight_SecondaryShield : HeroPowerUp
{
	private KnightHero knight;
	public bool shielded;

	[HideInInspector]
	public float chargePerSecond = 1;
	private float chargeCapacity = 20;
	private float charge;

	[Header("Animations")]
	public SimpleAnimation shieldBreakAnim;
	public SimpleAnimation shieldRegenAnim;
	public GameObject effect;
	[Header("Audio")]
	public AudioClip rechargeSound;
	public AudioClip breakSound;

	public delegate void OnShieldEvent();
	public event OnShieldEvent OnShieldBreak;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		knight.player.OnPlayerDamaged += AbsorbDamage;
		StartCoroutine(ChargeRoutine());
		ActivateShield ();
	}

	private void ActivateShield()
	{
		shielded = true;
		effect.SetActive (true);
		SoundManager.instance.RandomizeSFX(rechargeSound);
	}

	public void AbsorbDamage(int amt)
	{
		if (!shielded)
			return;

		// knight.player.Heal (Mathf.CeilToInt(amt / 2f));
		shielded = false;
		effect.GetComponent<IndicatorEffect> ().AnimateOut ();
		SoundManager.instance.RandomizeSFX(breakSound);

		if (OnShieldBreak != null)
			OnShieldBreak();
	}

	public override void Deactivate ()
	{
		knight.player.OnPlayerDamaged -= AbsorbDamage;
		base.Deactivate ();
	}

	public override void Stack ()
	{
		base.Stack ();
		chargePerSecond *= 1.5f;
	}

	private IEnumerator ChargeRoutine()
	{
		for (;;)
		{
			Charge(Time.deltaTime);
			yield return null;
		}
	}

	private void Charge(float amt)
	{
		if (shielded)
			return;
		charge += amt * chargePerSecond;
		percentActivated = charge / chargeCapacity;
		if (charge >= chargeCapacity)
		{
			ActivateShield ();
			charge = 0;
		}
	}
}

