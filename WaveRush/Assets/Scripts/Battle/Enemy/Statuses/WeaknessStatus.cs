using UnityEngine;
using System.Collections;

public class WeaknessStatus : EnemyStatus
{
	public SimpleAnimationPlayer anim;
	public SimpleAnimation hitAnim;

	public override void Init(Enemy enemy)
	{
		base.Init(enemy);
		
		anim.transform.localScale = Vector3.one * enemy.statusIconSize;
		anim.transform.localPosition = enemy.headPos;
	}

	protected override IEnumerator Effect()
	{
		anim.Play();
		enemy.OnEnemyDamaged += AdditionalDamage;
		while (timer > 0)
			yield return null;
		enemy.OnEnemyDamaged -= AdditionalDamage;
		Deactivate();
	}

	public override void Stack()
	{
		base.Stack();
		timer = duration;
	}

	private void AdditionalDamage(int amt)
	{
		enemy.DecreaseHealth(Mathf.RoundToInt(amt * 0.5f));
		EffectPooler.PlayEffect(hitAnim, transform.position, true);
	}
}