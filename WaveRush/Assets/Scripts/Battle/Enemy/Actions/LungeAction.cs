using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class LungeAction : EnemyAction
	{
		private Animator anim;
		private EntityPhysics body;
		private float defaultSpeed;
		private bool attacking = false;

		[Header("Properties")]
		public float chargeTime = 0.5f;
		public float attackTime = 0.2f;
		public float lungeSpeed = 10.0f;
		public int damage;
		[Header("AnimationStates")]
		public string chargeState;
		public string lungeState;
		[Header("Audio")]
		public AudioClip lungeSound;


		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			anim = e.anim;
			body = e.body;
			defaultSpeed = e.DEFAULT_SPEED;
		}

		public override void Execute()
		{
			base.Execute();
			StartCoroutine(UpdateState());
		}

		public override void Interrupt()
		{
			StopAllCoroutines();
			body.moveSpeed = defaultSpeed;
			body.Move(Vector3.zero);
			attacking = false;
		}

		private IEnumerator UpdateState()
		{
			// Charge up before attack
			Vector3 dir;
			Charge(out dir);
			yield return new WaitForSeconds(chargeTime);

			// Lunge
			Lunge(dir);
			yield return new WaitForSeconds(attackTime);

			// Reset vars
			attacking = false;
			body.moveSpeed = defaultSpeed;
			body.Move(dir.normalized);
			if (onActionFinished != null)
				onActionFinished();
		}

		private void Charge(out Vector3 dir)
		{
			//anim.ResetTrigger ("Charge");
			anim.CrossFade(chargeState, 0f);        // triggers are unreliable, crossfade forces state to execute
			body.Move(Vector2.zero);
			dir = (Vector2)(e.player.position - transform.position); // freeze moving direction
		}

		private void Lunge(Vector3 dir)
		{
			attacking = true;

			anim.CrossFade(lungeState, 0f);     // triggers are unreliable, crossfade forces state to execute
			SoundManager.instance.RandomizeSFX(lungeSound);

			body.moveSpeed = lungeSpeed;
			body.Move(dir.normalized);
		}

		protected void OnTriggerStay2D(Collider2D col)
		{
			if (col.CompareTag("Player"))
			{
				if (attacking)
				{
					Player player = col.GetComponentInChildren<Player>();
					player.Damage(damage);
				}
			}
		}
	}
}
