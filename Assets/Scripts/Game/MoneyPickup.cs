using UnityEngine;
using System.Collections;

public class MoneyPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
	private bool goToPlayer;
	private Transform player;
	public int value;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D> ();
	}

	void Start()
	{
		player = GameObject.Find ("Player").transform;
		rb2d.AddForce(new Vector2(Random.Range(-1f, 1f),
			Random.Range(-1f, 1f)),
			ForceMode2D.Impulse
		);
		Invoke ("StartGoToPlayer", Random.Range(1f, 2f));
	}

	private void StartGoToPlayer()
	{
		goToPlayer = true;
	}

	void Update()
	{
		if (goToPlayer)
		{
			float distance = Vector3.Distance (transform.position, player.position);
			Vector3 dir = (player.position - transform.position).normalized;
			rb2d.velocity = dir * distance * 5;
			if (distance <= 0.5)
			{
				GameManager.instance.wallet.Earn (value, false);
				Destroy (gameObject);
			}
		}
	}
}

