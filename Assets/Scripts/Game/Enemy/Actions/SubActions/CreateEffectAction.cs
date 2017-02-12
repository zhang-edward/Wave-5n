using UnityEngine;
using System.Collections;

public class CreateEffectAction : EnemyAction
{
	private ObjectPooler effectPool;

	public Transform location;
	public TempObjectInfo info;
	public SimpleAnimation anim;

	public override void Init(Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init(e, null);
		effectPool = ObjectPooler.GetObjectPooler("Effect");
	}

	public override void Execute()
	{
		base.Execute();
		TempObject effect = effectPool.GetPooledObject().GetComponent<TempObject>();
		SimpleAnimationPlayer effectAnim = effect.GetComponent<SimpleAnimationPlayer>();
		effectAnim.anim = anim;
		effect.Init(Quaternion.identity, location.position, anim.frames[0], info);
		effectAnim.Play();
	}

	public override void Interrupt()
	{
		if (!interruptable)
			return;
	}
}

