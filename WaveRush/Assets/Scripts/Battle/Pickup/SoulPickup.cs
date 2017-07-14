using UnityEngine;
using System.Collections;

public class SoulPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
	private bool goToPlayer;
	private Transform player;
	private ObjectPooler effectPool;

	public AudioClip soulPickupSound;
	public SimpleAnimation pickupEffect;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	public void Init(Transform player)
	{
		this.player = player;
		effectPool = ObjectPooler.GetObjectPooler("Effect");
		rb2d.AddForce(new Vector2(Random.Range(-3f, 3f),
			Random.Range(-3f, 3f)),
			ForceMode2D.Impulse
		);
		Invoke("StartGoToPlayer", Random.Range(2f, 4f));
	}

	private void StartGoToPlayer()
	{
		goToPlayer = true;
	}

	void Update()
	{
		if (goToPlayer)
		{
			float distance = Vector3.Distance(transform.position, player.position);
			Vector3 dir = (player.position - transform.position).normalized;
			rb2d.velocity = dir * Mathf.Pow(distance, 2) * 2;
			if (distance <= 1.0)
			{
				BattleSceneManager.instance.AddSouls(1);
				SoundManager.instance.PlaySingle(soulPickupSound);
				PlayEffect();
				Destroy(gameObject);
			}
		}
	}

	private void PlayEffect()
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, pickupEffect.TimeLength, 0, new Color(1, 1, 1, 0.8f));
		anim.anim = pickupEffect;
		tempObj.Init(Quaternion.identity,
					 transform.position,
				 pickupEffect.frames[0]);
		anim.Play();
	}
}

