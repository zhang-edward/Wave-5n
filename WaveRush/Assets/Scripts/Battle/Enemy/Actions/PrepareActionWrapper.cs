using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public abstract class PrepareActionWrapper : EnemyAction
	{
		protected AnimationSet anim;
		[Header("Times")]
		public float chargeTime = 0.5f;
		public float actionTime = 0.2f;
		public float endLagTime = 0.5f;
		[Header("AnimationStates")]
		public string chargeState;
		public string actionState;
		[Header("Audio")]
		public AudioClip actionSound;


		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			anim = e.anim;
		}

		public override void Execute()
		{
			base.Execute();
			StartCoroutine(UpdateState());
		}

		public override void Interrupt()
		{
			// print (this.gameObject + " interrupt");
			StopAllCoroutines();
		}

		private IEnumerator UpdateState()
		{
			// Charge up before attack
			Charge();
			yield return new WaitForSeconds(chargeTime);

			// Lunge
			Action();
			yield return new WaitForSeconds(actionTime);

			// Reset vars
			Reset();
			yield return new WaitForSeconds(endLagTime);

			// print (this.gameObject + "Finished");
			if (onActionFinished != null)
				onActionFinished();
		}

		protected virtual void Charge()
		{
			//anim.ResetTrigger ("Charge");
			anim.Play(chargeState);        // triggers are unreliable, crossfade forces state to execute
		}

		protected virtual void Action()
		{
			anim.Play(actionState);     // triggers are unreliable, crossfade forces state to execute
			SoundManager.instance.RandomizeSFX(actionSound);
		}

		protected abstract void Reset();
	}
}