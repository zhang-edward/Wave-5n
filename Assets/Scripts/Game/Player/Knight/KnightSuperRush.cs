using UnityEngine;
using System.Collections;

public class KnightSuperRush : HeroPowerUp
{
	private KnightHero knight;
	private float activateChance = 0.2f;

	public GameObject rushEffect;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.knight = (KnightHero)hero;
		knight.OnKnightRush += ActivateSuperRush;
		percentActivated = 0f;
	}

	public override void Deactivate()
	{
		base.Deactivate ();
		knight.OnKnightRush -= ActivateSuperRush;
	}

	public override void Stack ()
	{
		base.Stack ();
		activateChance += 0.08f;
	}

	private void ActivateSuperRush()
	{
		if (Random.value < activateChance)
		{
			knight.onSwipe -= knight.RushAbility;
			knight.onSwipe += SuperRush;
			percentActivated = 1f;
		}
	}

	private void SuperRush()
	{
		// check cooldown
		if (!knight.IsCooledDown (0, true, playerHero.HandleSwipe))
			return;
		knight.ResetCooldownTimer (0);

		// Sound
		SoundManager.instance.RandomizeSFX (knight.rushSound);
		// Animation
		knight.anim.SetBool ("Attacking", true);
		// Effects
		PlayRushEffect();
		// Player properties
		knight.damage = 2;
		knight.killBox = true;
		knight.body.moveSpeed = knight.baseRushMoveSpeed * 1.3f;
		knight.body.Move(playerHero.player.dir.normalized);
		// Debug.DrawRay (transform.position, playerHero.player.dir, Color.red, 0.5f);
		// reset ability
		Invoke ("ResetRushAbility", knight.baseRushDuration * 1.3f);

		knight.onSwipe -= SuperRush;
		knight.onSwipe += knight.RushAbility;
		percentActivated = 0f;
	}

	private void ResetRushAbility()
	{
		knight.damage = 1;
		knight.ResetRushAbility ();
	}

	private void PlayRushEffect()
	{
		TempObject effect = rushEffect.GetComponent<TempObject> ();
		SimpleAnimationPlayer animPlayer = rushEffect.GetComponent<SimpleAnimationPlayer> ();

		Vector2 dir = playerHero.player.dir;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		TempObjectInfo info = new TempObjectInfo ();
		info.targetColor = new Color (1, 1, 1, 0.5f);
		info.lifeTime = knight.baseRushDuration;
		info.fadeOutTime = 0.1f;
		effect.Init (Quaternion.Euler (new Vector3 (0, 0, angle)), transform.position, animPlayer.anim.frames [0], info);
		animPlayer.Play ();
	}
}

