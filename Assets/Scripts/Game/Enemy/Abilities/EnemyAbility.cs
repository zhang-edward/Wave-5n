using UnityEngine;
using System.Collections;

public abstract class EnemyAbility : MonoBehaviour
{
	public Sprite icon;
	protected Enemy enemy;

	public virtual void Init(Enemy enemy)
	{
		this.enemy = enemy;
	}
}

