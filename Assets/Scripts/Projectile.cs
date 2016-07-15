using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	protected Rigidbody2D rb2d;
	protected SpriteRenderer sr;
	protected BoxCollider2D box;
	public string target;

	public Sprite onShootEffect;
	public Sprite onCollideEffect;
	public SimpleAnimation onCollideAnim;
	protected ObjectPooler effectPool;

	protected int damage;
	//private int speed;


	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
		sr = this.GetComponent<SpriteRenderer> ();
		box = this.GetComponent<BoxCollider2D> ();
	}

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
	}

	public void DestroySelf()
	{
		gameObject.SetActive(false);
	}

	public void Init(Vector3 pos, Vector2 dir, Sprite sprite, string target, float speed = 4, int damage = 1)
	{
		gameObject.SetActive (true);
		transform.position = pos;
		rb2d.velocity = dir.normalized * speed;
		sr.sprite = sprite;
		// set the collider size to match the sprite
		box.size = sprite.bounds.size;
		// Set angle to be facing the direction of motion
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3 (0, 0, angle);

		this.target = target;
		this.damage = damage;
		Invoke ("DestroySelf", 5.0f);
	}

	void OnDisable()
	{
		// prevent the invoke from continuing after this object has been disabled already
		CancelInvoke ();
	}

	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag(target))
		{
			//Debug.Log (col);
			IDamageable damageableTarget = col.GetComponentInChildren<IDamageable> ();
			damageableTarget.Damage (damage);
			OnCollide ();
		}
	}

	protected void OnCollide()
	{
		if (onCollideEffect != null)
		{
			TempObject effect = effectPool.GetPooledObject ().GetComponent<TempObject>();
			effect.Init (
				Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))),
				transform.position,
				onCollideEffect,
				true
			);
		}
		else if (onCollideAnim != null)
		{
			TempObject effect = effectPool.GetPooledObject ().GetComponent<TempObject>();
			SimpleAnimationPlayer animPlayer = effect.GetComponent<SimpleAnimationPlayer> ();

			animPlayer.anim = onCollideAnim;
			effect.Init (
				Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))),
				transform.position,
				onCollideEffect,
				true,
				0,
				animPlayer.anim.TimeLength,
				0.2f
			);
			animPlayer.Play ();
		}
	}
}

