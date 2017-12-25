namespace EnemyActions
{
	using UnityEngine;
	using Projectiles;
	using System.Collections;

	public class EA_ShootAtTarget : EnemyAction
	{
		protected Animator anim;
		protected EntityPhysics body;
		protected ObjectPooler projectilePool;
		protected Vector3 shootPointPos;
		[Header("Set from Hierarchy")]
		public GenerateRandomPositionNearPlayerAction posGenerator;
		public GameObject projectilePrefab;
		public Transform shootPoint;
		[Header("Properties")]
		public float chargeTime = 0.5f;
		public float attackTime = 0.2f;
		public bool reflectX = true;
		[Header("AnimationStates")]
		public string chargeState;
		public string shootState;
		[Header("Audio")]
		public AudioClip shootSound;

		public event OnActionStateChanged onShoot;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			anim = e.anim;
			body = e.body;
			projectilePool = projectilePrefab.GetComponent<Projectile>().GetObjectPooler();
			shootPointPos = shootPoint.localPosition;
		}

		public override void Execute()
		{
			base.Execute();
			StartCoroutine(UpdateState());
		}

		public override void Interrupt()
		{
			StopAllCoroutines();
		}

		private IEnumerator UpdateState()
		{
			//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
			//Debug.Log ("attacking: enter");
			// Charge up before attack
			Charge();

			// adjust for player hitbox
			Vector2 dir = e.player.transform.position - shootPoint.position;
			yield return new WaitForSeconds(chargeTime);

			if (e.sr.flipX)
				shootPoint.localPosition = new Vector3(shootPointPos.x * -1, shootPointPos.y);
			else
				shootPoint.localPosition = new Vector3(shootPointPos.x, shootPointPos.y);

			// Shoot
			Shoot(dir.normalized);

			if (onShoot != null)
				onShoot();
			yield return new WaitForSeconds(attackTime);

			if (onActionFinished != null)
				onActionFinished();
		}

		private void Charge()
		{
			anim.CrossFade(chargeState, 0f);        // triggers are unreliable, crossfade forces state to execute
			body.Move(Vector2.zero);
			//+ new Vector2(Random.value, Random.value);		// add a random offset
		}

		protected virtual void Shoot(Vector2 dir)
		{
			anim.CrossFade(shootState, 0f);     // triggers are unreliable, crossfade forces state to execute
			Projectile p = projectilePool.GetPooledObject().GetComponent<Projectile>();
			float lifeTime = Vector3.Distance(shootPoint.position, posGenerator.GetGeneratedPosition()) / p.speed;
			p.lifeTime = lifeTime;
			UnityEngine.Assertions.Assert.IsNotNull(p);
			p.Init(shootPoint.position, dir);
			SoundManager.instance.RandomizeSFX(shootSound);
		}
	}
}
