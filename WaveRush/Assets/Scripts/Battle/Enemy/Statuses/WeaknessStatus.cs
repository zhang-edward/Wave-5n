using UnityEngine;
using System.Collections;

public class WeaknessStatus : EnemyStatus
{
	private ObjectPooler effectPool;

	public SimpleAnimationPlayer anim;
	public SimpleAnimation hitAnim;

	public override void Init(Enemy enemy)
	{
		base.Init(enemy);
		effectPool = ObjectPooler.GetObjectPooler("Effect");
		anim.transform.localScale = enemy.srSize * 0.8f;
		anim.transform.localPosition = enemy.healthBarOffset * 0.5f;
	}

	protected override IEnumerator Effect()
	{
		anim.Play();
		enemy.OnEnemyDamaged += DoubleDamage;
		while (timer > 0)
			yield return null;
		enemy.OnEnemyDamaged -= DoubleDamage;
		Destroy(gameObject);
	}

	public override void Stack()
	{
		base.Stack();
		timer = duration;
	}

	private void DoubleDamage(int amt)
	{
		enemy.DecreaseHealth(amt);
		PlayEffect();
	}

	private void PlayEffect()
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, hitAnim.TimeLength, 0);
		anim.anim = hitAnim;
		tempObj.Init(Quaternion.Euler(0, 0, Random.Range(0, 360)),
		             transform.position,
					 hitAnim.frames[0]);
		anim.Play();
	}
}