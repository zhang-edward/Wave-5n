using UnityEngine;
using System.Collections;

public class SoulPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
	private bool goToPlayer;
	private Transform player;

	public AudioClip soulPickupSound;
	public SimpleAnimation pickupEffect;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	public void Init(Transform player)
	{
		this.player = player;
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
				EffectPooler.PlayEffect(pickupEffect, transform.position);
				Destroy(gameObject);
			}
		}
	}
}

