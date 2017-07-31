using UnityEngine;
using System.Collections;

public class KnightMassStun : HeroPowerUp
{
	private KnightHero knight;
	private Vector3 explosionPos;

	[Header("Audio")]
	public AudioClip effectSound;
	public AudioClip attackSound;
	[Header("Animations")]
	public SimpleAnimation massStunExplosionAnim;
	public SimpleAnimation hitAnim;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		
		this.knight = (KnightHero)hero;
		knight.onSpecialAbility += MassStun;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	public override void Stack()
	{
		base.Stack();
	}

	private void MassStun()
	{
		StartCoroutine(MassStunRoutine());
	}

	private IEnumerator MassStunRoutine()
	{
		EffectPooler.PlayEffect(massStunExplosionAnim, transform.position);
		explosionPos = transform.position;
		SoundManager.instance.RandomizeSFX(effectSound);
		float delay = massStunExplosionAnim.SecondsPerFrame * 9f; // wait until frame 9
		yield return new WaitForSeconds(delay);
		CameraControl.instance.StartFlashColor(Color.white, 1, 0, 0.1f, 0.5f);
		SoundManager.instance.RandomizeSFX(attackSound);
		AreaAttack();
	}

	private void AreaAttack()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 99);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy>();

				GameObject stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun"));
				stun.GetComponent<StunStatus>().duration = 10f;
				e.AddStatus(stun.gameObject);
				EffectPooler.PlayEffect(hitAnim, e.transform.position);
			}
		}
	}
}

