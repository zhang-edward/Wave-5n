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
		enemy.body.AddRandomImpulse(3f);
		anim.Play();
		while (timer > 0)
		{
			enemy.action.Interrupt();
			enemy.Disable(0);
			yield return null;
		}
		Destroy(gameObject);
	}

	public override void Stack()
	{
		base.Stack();
		StopAllCoroutines();
		StartCoroutine(Effect());
	}
}