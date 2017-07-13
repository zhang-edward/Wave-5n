using UnityEngine;
using System.Collections;

public class SoulPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
	private bool goToPlayer;
	private Transform player;
	public int value;

	public AudioClip coinPickupSound;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	public void Init(Transform player)
	{
		this.player = player;

		rb2d.AddForce(new Vector2(Random.Range(-1f, 1f),
			Random.Range(-1f, 1f)),
			ForceMode2D.Impulse
		);
		Invoke("StartGoToPlayer", Random.Range(1f, 2f));
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
			rb2d.velocity = dir * distance * 2;
			if (distance <= 2.0)
			{
				BattleSceneManager.instance.AddMoney(value);
				SoundManager.instance.PlaySingle(coinPickupSound);
				Destroy(gameObject);
			}
		}
	}
}

