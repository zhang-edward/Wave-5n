using UnityEngine;
using System.Collections;

public class EnemyAbility : MonoBehaviour
{
	protected Enemy enemy;

	public virtual void Init(Enemy enemy)
	{
		this.enemy = enemy;
	}
}

