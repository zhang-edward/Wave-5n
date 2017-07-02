using UnityEngine;
using System.Collections;

public class KnightMassStun : HeroPowerUp
{
	private KnightHero knight;
	private ObjectPooler effectPool;
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
		effectPool = ObjectPooler.GetObjectPooler("Effect");
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
		PlayEffect();
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
				PlayHitEffect(e.transform.position);
			}
		}
	}

	private void PlayEffect()
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, massStunExplosionAnim.TimeLength, 0, new Color(1, 1, 1, 0.8f));
		anim.anim = massStunExplosionAnim;
		tempObj.Init(Quaternion.identity,
					 transform.position,
				 massStunExplosionAnim.frames[0]);
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

