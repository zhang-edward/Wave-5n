using UnityEngine;
using System.Collections;

public class MoneyPickup : MonoBehaviour
{
	private Rigidbody2D rb2d;
	private bool goToPlayer;
	private Transform player;
	public int value;

	public AudioClip coinPickupSound;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D> ();
	}

	void Start()
	{
		GameObject o = GameObject.Find ("Player");
		// if cannot find player, player has died already;
		// any coins spawned after death should be destroyed
		if (o == null)
			Destroy (this);
		else
			player = o.transform;
		
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
			if (distance <= 1.0)
			{
				GameManager.instance.wallet.Earn (value, false);
				SoundManager.instance.PlaySingle (coinPickupSound);
				Destroy (gameObject);
			}
		}
	}
}

