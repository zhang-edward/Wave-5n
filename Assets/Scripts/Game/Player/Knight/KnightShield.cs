using UnityEngine;
using System.Collections;

public class KnightShield : HeroPowerUp
{
	public KnightHero knight;
	public bool shielded;
	public float chargeCapacity = 50;

	[HideInInspector]
	public float chargePerHit = 1;
	private float charge;

	[Header("Animations")]
	public SimpleAnimation shieldBreakAnim;
	public SimpleAnimation shieldRegenAnim;
	public TempObject effect;

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
		PlayEffect (shieldRegenAnim);
	}

	public void AbsorbDamage(int amt)
	{
		if (!shielded)
			return;

		knight.player.Heal (amt);
		Deactivate ();
		PlayEffect (shieldBreakAnim);
		CameraControl.instance.StartShake (0.1f, 0.05f);
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		shielded = false;
	}

	public override void Stack ()
	{
		base.Stack ();
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

	private void PlayEffect(SimpleAnimation anim)
	{
		SimpleAnimationPlayer animPlayer = effect.GetComponent<SimpleAnimationPlayer> ();
		animPlayer.anim = anim;

		TempObjectInfo info = new TempObjectInfo ();
		info.isSelfDeactivating = true;
		info.lifeTime = animPlayer.anim.TimeLength + 0.5f;
		info.targetColor = new Color (1, 1, 1, 1f);
		effect.Init (
			Quaternion.identity,
			transform.position,
			animPlayer.anim.frames[0],
			info
		);
		animPlayer.Play ();
	}
}

