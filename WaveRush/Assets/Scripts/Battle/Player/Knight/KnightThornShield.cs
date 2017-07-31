using UnityEngine;
using System.Collections;

public class KnightThornShield : HeroPowerUp
{
	private float radius = 2;
	private KnightShield shieldPowerUp;
	private Vector3 shieldBreakPos;

	[Header("Animations")]
	public SimpleAnimation thornShieldAnim;
	public SimpleAnimation hitAnim;
	[Header("Audio")]
	public AudioClip effectSound;
	public AudioClip attackSound;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		
		shieldPowerUp = hero.GetComponentInChildren<KnightShield>();
		shieldPowerUp.OnShieldBreak += ThornShield;
	}

	private void ThornShield()
	{
		StartCoroutine(ThornShieldRoutine());
	}

	private IEnumerator ThornShieldRoutine()
	{
		EffectPooler.PlayEffect(thornShieldAnim, transform.position);
		shieldBreakPos = transform.position;
		SoundManager.instance.RandomizeSFX(effectSound);
		float delay = thornShieldAnim.SecondsPerFrame * 9f; // wait until frame 9
		yield return new WaitForSeconds(delay);
		AreaAttack();
		SoundManager.instance.RandomizeSFX(attackSound);
	}

	private void AreaAttack()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll(shieldBreakPos, radius);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy>();

				GameObject stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun"));
				e.AddStatus(stun.gameObject);
				EffectPooler.PlayEffect(hitAnim, e.transform.position, true);
			}
		}
	}
}