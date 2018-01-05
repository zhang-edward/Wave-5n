using UnityEngine;
using System.Collections;

public class Mage_Volcano : HeroPowerUp
{
	private const float RADIUS = 4f;
	private float activateChance = 0.5f;
	private MageHero mage;

	[Header("Animations")]
	public SimpleAnimation eruptionAnim;
	[Header("Audio")]
	public AudioClip[] groundBreakSounds;
	public AudioClip[] eruptionSounds;


	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
		mage.OnMageTeleportIn += ActivateEffect;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	public override void Stack()
	{
		base.Stack();
		activateChance += 0.8f;
	}

	private void ActivateEffect()
	{
		if (Random.value > activateChance)
			return;
		Enemy e = null;
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, RADIUS);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				e = col.gameObject.GetComponentInChildren<Enemy>();
				StartCoroutine(Eruption(e));
			}
		}
	}

	private IEnumerator Eruption(Enemy e)
	{
		float frame11time = eruptionAnim.SecondsPerFrame * 11f;
		StunEnemy(e, frame11time);
		EffectPooler.PlayEffect(eruptionAnim, e.transform.position, false, 0.3f);

		SoundManager.instance.RandomizeSFX(groundBreakSounds[Random.Range(0, groundBreakSounds.Length)]);
		yield return new WaitForSeconds(frame11time);

		SoundManager.instance.RandomizeSFX(eruptionSounds[Random.Range(0, eruptionSounds.Length)]);
		e.Damage(Mathf.RoundToInt(mage.damage * 2.5f));
		CameraControl.instance.StartShake(0.2f, 0.05f, true, false);
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}
