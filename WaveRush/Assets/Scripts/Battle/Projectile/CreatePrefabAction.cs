using UnityEngine;
namespace Projectiles
{
	public class CreatePrefabAction : ProjectileAction
	{
		public GameObject prefab;

		public override void Execute()
		{
			Instantiate(prefab, transform.position, Quaternion.identity);
		}
	}
}
