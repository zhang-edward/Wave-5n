using UnityEngine;

public class AnimationAction : EnemyAction
{
	[System.Serializable]
	public class AnimationState
	{
		public string stateName;
		public float duration;
	}
	public AnimationState[] states;

	public override void Init(Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init(e, onActionFinished);
	}
}