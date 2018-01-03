using UnityEngine;
using System.Collections;

public class StunStatus : EnemyStatus
{
	public SimpleAnimationPlayer anim;

	public override void Init(Enemy enemy)
	{
		base.Init(enemy);
		anim.transform.localScale = enemy.srSize * 0.8f;
		anim.transform.localPosition = enemy.healthBarOffset * 0.3f;
	}

	protected override IEnumerator Effect()
	{
		anim.Play();
		enemy.body.AddRandomImpulse(3f);
		while (timer > 0)
		{
			enemy.Disable(0);
			enemy.action.Interrupt();
			yield return null;
		}
		Deactivate();
	}

	public override void Stack()
	{
		base.Stack();
		StopAllCoroutines();
		StartCoroutine(Effect());
	}
}