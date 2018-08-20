namespace EnemyActions
{
	using UnityEngine;
	using Projectiles;
	using System.Collections;

	public class EA_Shoot : PrepareActionWrapper
	{
		protected EntityPhysics body;
		protected ObjectPooler projectilePool;
		protected Vector3 shootPointPos;
		[Header("Set from Hierarchy")]
		public GameObject projectilePrefab;
		public Transform shootPoint;
		[Header("Properties")]
		public bool reflectX = true;
		
		protected Vector2 dir;

		public event OnActionStateChanged onShoot;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			body = e.body;
			projectilePool = projectilePrefab.GetComponent<Projectile>().GetObjectPooler();
			shootPointPos = shootPoint.localPosition;
		}

		protected override void Charge() {
			base.Charge();
			body.Move(Vector2.zero);

			dir = e.playerTransform.transform.position - shootPoint.position;
			if (e.sr.flipX)
				shootPoint.localPosition = new Vector3(shootPointPos.x * -1, shootPointPos.y);
			else
				shootPoint.localPosition = new Vector3(shootPointPos.x, shootPointPos.y);

		}

		protected override void Action() {
			base.Action();
			Projectile p = projectilePool.GetPooledObject().GetComponent<Projectile>();
			UnityEngine.Assertions.Assert.IsNotNull(p);
			p.Init(shootPoint.position, dir, e);
			if (onShoot != null)
				onShoot();
		}

		protected override void Reset() {
			dir = Vector2.zero;
		}
	}
}
