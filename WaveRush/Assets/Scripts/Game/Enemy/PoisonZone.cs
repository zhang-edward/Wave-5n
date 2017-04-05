using UnityEngine;

public class PoisonZone : MonoBehaviour
{
	public float radius;		// the size of the zone
	public float interval;      // how often this zone damages the player
	public int damage;
	public float duration = 10f;

	public SpriteRenderer sr;
	public Sprite[] sprites;

	private float timer = 0;

	void Start()
	{
		sr.sprite = sprites[Random.Range(0, sprites.Length)];
		Destroy(this.gameObject, duration);
	}

	void Update()
	{
		timer -= Time.deltaTime;
		if (timer > 0)
			return;
		
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
				col.GetComponentInChildren<Player>().Damage(damage);
				timer = interval;
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}