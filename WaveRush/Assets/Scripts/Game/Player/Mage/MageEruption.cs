using UnityEngine;
using System.Collections;

public class MageEruption : HeroPowerUp
{
	private const float RADIUS = 4f;
	private ObjectPooler effectPool;
	private float activateChance = 0.5f;
	private MageHero mage;

	[Header("Animations")]
	public SimpleAnimation eruptionAnim;
	[Header("Audio")]
	public AudioClip[] groundBreakSounds;
	public AudioClip[] eruptionSounds;

	void Awake()
	{
		effectPool = ObjectPooler.GetObjectPooler("Effect");
	}

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
				break;
			}
		}
	}

	private IEnumerator Eruption(Enemy e)
	{
		float frame11time = eruptionAnim.SecondsPerFrame * 11f; // time before the sword rises up in the animation
		StunEnemy(e, frame11time);
		PlayEffect(eruptionAnim, e.transform.position, 0.3f);

		SoundManager.instance.RandomizeSFX(groundBreakSounds[Random.Range(0, groundBreakSounds.Length)]);
		yield return new WaitForSeconds(frame11time);

		SoundManager.instance.RandomizeSFX(eruptionSounds[Random.Range(0, eruptionSounds.Length)]);
		e.Damage(3);
		CameraControl.instance.StartShake(0.2f, 0.05f);
	}

	private void PlayEffect(SimpleAnimation toPlay, Vector3 position, float fadeOutTime)
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, toPlay.TimeLength - fadeOutTime, fadeOutTime, new Color(1, 1, 1, 0.8f));
		anim.anim = toPlay;
		tempObj.Init(Quaternion.identity,
					 position,
					 toPlay.frames[0]);
		anim.Play();
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}
