using UnityEngine;
using System.Collections;
namespace Projectiles
{
	public class Projectile : MonoBehaviour
	{
		private static string OBJECT_POOLER_TAG = "Projectile_";
		protected Rigidbody2D rb2d;
		protected SpriteRenderer sr;
		protected SimpleAnimationPlayer animator;
		protected ObjectPooler projectilePool;
		protected BoxCollider2D box;

		[Header("Properties")]
		[Tooltip("This MUST be unique for each projectile!")]
		public string projectileName;       // unique identifier for this particular prefab
		public string target;
		public float speed;
		public float lifeTime;
		public bool setAngle = true;
		public bool destroyOnCollide;
		public Vector2 size;
		public SimpleAnimation projectileAnim;
		public IDamageable source;

		public delegate void ProjectileLifeCycleEvent();
		public event ProjectileLifeCycleEvent OnShoot;
		public event ProjectileLifeCycleEvent OnFlight;
		public event ProjectileLifeCycleEvent OnCollide;
		public event ProjectileLifeCycleEvent OnDie;

		public delegate void DamagedTarget(IDamageable damageable, int damage);
		public event DamagedTarget OnDamagedTarget;
		public delegate void CollidedTarget(IDamageable damageable);
		public event CollidedTarget OnCollidedTarget;
		// [Header("Effects")]
		// public SimpleAnimation onShootAnim, inFlightAnim, onCollideAnim;
		// public TempObjectInfo onShootEffect, onCollideEffect;

		void Awake()
		{
			rb2d = GetComponent<Rigidbody2D>();
			sr = GetComponent<SpriteRenderer>();
			box = GetComponent<BoxCollider2D>();
			animator = GetComponent<SimpleAnimationPlayer>();
			// set the collider size to match the sprite
			box.size = size;
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireCube(transform.position, size);
		}

		public ObjectPooler GetObjectPooler()
		{
			// get the unique object pooler for this projectile
			projectilePool = ObjectPooler.GetObjectPooler(OBJECT_POOLER_TAG + projectileName);
			// if object pooler doesn't exist
			if (projectilePool == null)
			{
				// print("ObjectPooler with name " + OBJECT_POOLER_TAG + projectileName + " doesn't exist. Creating a new one...");
				projectilePool = RuntimeObjectPoolerManager.instance.CreateRuntimeObjectPooler(OBJECT_POOLER_TAG + projectileName, this.gameObject);
			}
			return projectilePool;
		}

		public void DestroySelf()
		{
			if (OnDie != null)
			{
				OnDie();
			}
			sr.enabled = false;
			StartCoroutine(DestroySelfRoutine());
		}

		public IEnumerator DestroySelfRoutine()
		{
			ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
			if (particles != null)
			{
				while (particles.isPlaying)
					yield return null;
			}
			sr.enabled = true;
			gameObject.SetActive(false);
		}

		public void Init(Vector3 pos, Vector2 dir, IDamageable src)
		{
			source = src;
			gameObject.SetActive(true);

			transform.position = pos;
			SetVelocity(dir);
			// Set angle to be facing the direction of motion
			if (setAngle)
			{
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, 0, angle);
			}

			animator.looping = true;
			animator.Play(projectileAnim);

			// Event
			if (OnShoot != null)
				OnShoot();
			StartCoroutine(OnFlightRoutine());

			Invoke("DestroySelf", lifeTime);
		}

		public void SetVelocity(Vector2 dir)
		{
			rb2d.velocity = dir.normalized * speed;
		}

		void OnDisable()
		{
			// prevent the invoke from continuing after this object has been disabled already
			CancelInvoke();
		}

		protected virtual void OnTriggerEnter2D(Collider2D col)
		{
			if (col.CompareTag(target))
			{
				// Event
				if (OnCollide != null)
					OnCollide();
				if (OnCollidedTarget != null)
					OnCollidedTarget(col.GetComponentInChildren<IDamageable>());

				if (destroyOnCollide)
					gameObject.SetActive(false);
			}
		}

		public void DamageTarget(IDamageable damageable, int baseDamage)
		{
			// Check if the source of this projectile was an enemy and if so, scale damage accordingly
			Enemy e = source as Enemy;
			int damage;
			if (e != null) {
				damage = Formulas.EnemyDamageFormula(baseDamage, e.GetLevelDiff());
			}
			else {
				damage = baseDamage;
			}

			damageable.Damage(damage, source);
			// Event
			if (OnDamagedTarget != null)
				OnDamagedTarget(damageable, damage);
		}

		private IEnumerator OnFlightRoutine()
		{
			while (gameObject.activeInHierarchy)
			{
				if (OnFlight != null)
					OnFlight();
				yield return null;
			}
		}
	}
}