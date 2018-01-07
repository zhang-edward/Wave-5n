namespace PlayerActions
{
	using UnityEngine;

	[System.Serializable]
	public class PA_Effect : PlayerAction
	{
		/** Set in Inspector */
		public SimpleAnimation effect;

		/** Properties */
		protected enum RotationType
		{
			None,                   // Rotation is Quaternion.identity
			Same,                   // Rotation matches player dir
			Opposite,               // Rotation is reverse player dir
			MatchFlipX,             // Match the player sprite's flipX
			ReverseFlipX,           // Match the reverse of the player sprite's flipX
			Random					// Rotation is random
		}
		[SerializeField] protected RotationType rotationType = RotationType.None; // Default value doesn't matter, just prevents warning
		[SerializeField] protected Color color = Color.white;
		private Vector3 position;

		public override void Init(Player player)
		{
			base.Init(player);
		}

		protected override void DoAction()
		{
			// Set the rotation for the effect
			Quaternion rot = GetRotation();

			// Initialize the effect properties
			TempObjectInfo info = new TempObjectInfo();
			info.targetColor = color;
			if (duration < 0)
				duration = effect.TimeLength;
			info.lifeTime = duration;
			info.fadeOutTime = 0.1f;

			EffectPooler.PlayEffect(effect, position, info);
		}

		protected Quaternion GetRotation()
		{
			float angle;
			switch (rotationType)
			{
				case RotationType.Same:
					angle = Mathf.Atan2(player.dir.y, player.dir.x) * Mathf.Rad2Deg;
					break;
				case RotationType.Opposite:
					angle = Mathf.Atan2(-player.dir.y, -player.dir.x) * Mathf.Rad2Deg;
					break;
				case RotationType.MatchFlipX:
					angle = player.sr.flipX ? 180 : 0;
					break;
				case RotationType.ReverseFlipX:
					angle = player.sr.flipX ? 0 : 180;
					break;
				case RotationType.Random:
					angle = Random.Range(0, 360);
					break;
				default:
					angle = 0;
					break;
			}
			return Quaternion.Euler(new Vector3(0, 0, angle));
		}

		public void SetPosition(Vector3 pos)
		{
			position = pos;
		}
	}
}
