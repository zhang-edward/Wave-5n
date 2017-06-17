using UnityEngine;
using System.Collections;

public class KnightShield : HeroPowerUp
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

		knight.player.Heal (amt / 2);
		shielded = false;
		effect.GetComponent<IndicatorEffect> ().AnimateOut ();
		CameraControl.instance.StartShake (0.1f, 0.05f);
		SoundManager.instance.RandomizeSFX(breakSound);

		if (OnShieldBreak != null)
			OnShieldBreak();
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
	}

	public override void Stack ()
	{
		base.Stack ();
		chargePerSecond *= 1.5f;
	}

	private IEnumerator ChargeRoutine()
	{
		while (true)
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

