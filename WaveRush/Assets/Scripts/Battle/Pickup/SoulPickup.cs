using UnityEngine;
using System.Collections;

public class SoulPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
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
		StartCoroutine(GoToPlayer());
	}

	private IEnumerator GoToPlayer()
	{
		yield return new WaitForSeconds(Random.Range(1f, 2f));
		float distance = Vector3.Distance(transform.position, player.position);
		while (distance > 2.0f)
		{
			distance = Vector3.Distance(transform.position, player.position);
			Vector3 dir = (player.position - transform.position).normalized;
			rb2d.velocity = dir * distance * 3;
			yield return null;
		}
		BattleSceneManager.instance.AddSouls(1);
		SoundManager.instance.PlaySingle(soulPickupSound);
		EffectPooler.PlayEffect(pickupEffect, transform.position);
		Destroy(gameObject);
	}
}

