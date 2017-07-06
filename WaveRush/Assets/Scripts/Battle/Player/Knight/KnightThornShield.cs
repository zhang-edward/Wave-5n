using UnityEngine;
using System.Collections;

public class KnightThornShield : HeroPowerUp
{
	private float radius = 2;
	private KnightShield shieldPowerUp;
	private ObjectPooler effectPool;
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
		effectPool = ObjectPooler.GetObjectPooler("Effect");
		shieldPowerUp = hero.GetComponentInChildren<KnightShield>();
		shieldPowerUp.OnShieldBreak += ThornShield;
	}

	private void ThornShield()
	{
		StartCoroutine(ThornShieldRoutine());
	}

	private IEnumerator ThornShieldRoutine()
	{
		PlayEffect();
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
				PlayHitEffect(e.transform.position);
			}
		}
	}

	private void PlayEffect()
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, thornShieldAnim.TimeLength, 0, new Color(1, 1, 1, 0.8f));
		anim.anim = thornShieldAnim;
		tempObj.Init(Quaternion.identity,
					 transform.position,
			     thornShieldAnim.frames[0]);
		anim.Play();
	}

	private void PlayHitEffect(Vector3 position)
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, hitAnim.TimeLength, 0);
		anim.anim = hitAnim;
		tempObj.Init(Quaternion.Euler(0, 0, Random.Range(0, 360)),
					 position,
			     hitAnim.frames[0]);
		anim.Play();
	}
}