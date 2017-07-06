using UnityEngine;
using System.Collections;


namespace EnemyActions
{
	public class SwitchMoveStateAction : EnemyAction
	{
		public MoveState conditionalMoveState;
		[Header("Effect (Optional)")]
		public SimpleAnimation switchEffect;

		private ObjectPooler effectPool;
		private bool isConditionalState;

		public override void Init(Enemy e, EnemyAction.OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			effectPool = ObjectPooler.GetObjectPooler("Effect");
			conditionalMoveState.Init(e, e.player);
		}

		public override void Execute()
		{
			base.Execute();
			e.movementMethod = conditionalMoveState;
			if (switchEffect.frames.Length > 0)
				PlayEffect(switchEffect, e.transform.position + e.healthBarOffset * 1.1f, 0.2f);
			if (onActionFinished != null)
				onActionFinished();
		}

		public override bool CanExecute()
		{
			isConditionalState = e.movementMethod == conditionalMoveState;
			return base.CanExecute() && !isConditionalState;
		}

		public override void Interrupt()
		{
		}

		private void PlayEffect(SimpleAnimation toPlay, Vector3 position, float fadeOutTime)
		{
			GameObject o = effectPool.GetPooledObject();
			SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
			TempObject tempObj = o.GetComponent<TempObject>();
			tempObj.info = new TempObjectInfo(true, 0f, toPlay.TimeLength - fadeOutTime, fadeOutTime, new Color(1, 1, 1, 0.8f));
			anim.anim = toPlay;
			tempObj.Init(Quaternion.identity,
						 position,
						 toPlay.frames[0]);
			anim.Play();
		}
	}
}
