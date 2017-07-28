namespace PlayerAbilities
{
	using UnityEngine;
	using System.Collections;

	public class PlayerAbility : MonoBehaviour
	{
		protected Player player;
		protected PlayerHero hero;

		public virtual void Init(Player player)
		{
			this.player = player;
			this.hero = player.hero;
		}

	}
}
