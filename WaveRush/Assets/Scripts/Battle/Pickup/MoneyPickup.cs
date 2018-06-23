using UnityEngine;
using System.Collections;

public class MoneyPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
	private Transform player;
	private SimpleAnimationPlayer anim;
	public int value;

	public AudioClip coinPickupSound;
	public SimpleAnimation pickupEffect;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D> ();
		anim = GetComponent<SimpleAnimationPlayer>();
	}

	void OnEnable() {
		anim.Play();
	}

	public void Init(Transform player)
	{
		this.player = player;
		rb2d.AddForce(new Vector2(Random.Range(-1f, 1f),
			Random.Range(-1f, 1f)),
			ForceMode2D.Impulse
		);
		StartCoroutine(GoToPlayer());
	}

	private IEnumerator GoToPlayer()
	{
		yield return new WaitForSeconds(Random.Range(1f, 2f));
		float distance = Vector3.Distance(transform.position, player.position);
		while (distance > 1.0f)
		{
			distance = Vector3.Distance (transform.position, player.position);
			Vector3 dir = (player.position - transform.position).normalized;
			rb2d.velocity = dir * distance * 5;
			yield return null;
		}
		BattleSceneManager.instance.AddMoney(this.value);
		SoundManager.instance.PlaySingle (coinPickupSound);
		EffectPooler.PlayEffect(pickupEffect, transform.position);
		gameObject.SetActive(false);
	}
}

