namespace PlayerActions
{
	using UnityEngine;

	[System.Serializable]
	public class PA_Move : PlayerAction
	{
		/** Properties */
		[SerializeField] private float speed;		// How fast the player should move
		[SerializeField] private float time;		// How long the player should move at the specified speed
		[SerializeField] private float drag;		// How fast the player should slow down after he has stopped moving
		private Vector3 direction;

		public void SetDirection(Vector3 dir) {
			direction = dir;
		}

		protected override void DoAction() {
			player.body.rb2d.drag = drag;
			player.body.moveSpeed = speed;
			player.body.Move(direction.normalized, time);
		}
	}
}
