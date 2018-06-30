using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Mage_Volcano : HeroPowerUp
{
	private const int   ERUPTION_FRAME = 11;
	private const float RADIUS 		   = 4f;

	private PyroHero mage;
	private Queue<Enemy> enemiesToAttack = new Queue<Enemy>();

	[Header("Animations")]
	public PA_EffectCallback eruption;
	[Header("Audio")]
	public AudioClip[] groundBreakSounds;
	public AudioClip[] eruptionSounds;


	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (PyroHero)hero;
		mage.OnPyroTeleportDamagedEnemy += ActivateEffect;
		eruption.Init(hero.player, ERUPTION_FRAME);
		eruption.onFrameReached += HandleEruption;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		mage.OnPyroTeleportDamagedEnemy -= ActivateEffect;
	}

	private void ActivateEffect()
	{
		List<Enemy> enemies = new List<Enemy>();
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, RADIUS);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
				if (e.GetStatus("Burn") != null)
					enemies.Add(e);
			}
		}
		StartCoroutine(EruptionAll(enemies));
	}

	private IEnumerator EruptionAll(List<Enemy> enemies)
	{
		foreach (Enemy e in enemies)
		{
			Eruption(e);
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void Eruption(Enemy e)
	{
		// Make enemy stand in place
		float actionDelay = eruption.effect.SecondsPerFrame * ERUPTION_FRAME;
		e.Disable(actionDelay);
		// Execute eruption
		eruption.SetPosition(e.transform.position + e.feetPos);
		eruption.Execute();
		// Add enemies to list to be attacked later
		enemiesToAttack.Enqueue(e);
		// Sound
		SoundManager.instance.RandomizeSFX(groundBreakSounds[Random.Range(0, groundBreakSounds.Length)]);
	}

	private void HandleEruption(int frame)
	{
		if (frame != ERUPTION_FRAME)
			return;
		Enemy e = enemiesToAttack.Dequeue();
		e.Damage(Mathf.RoundToInt(mage.damage * 2f), playerHero.player);
		e.GetStatus("Burn").Deactivate();
		CameraControl.instance.StartShake(0.2f, 0.05f, true, false);
		SoundManager.instance.RandomizeSFX(eruptionSounds[Random.Range(0, eruptionSounds.Length)]);
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}
