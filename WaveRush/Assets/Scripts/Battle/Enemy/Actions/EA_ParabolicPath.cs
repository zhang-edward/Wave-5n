namespace EnemyActions {
	using UnityEngine;
	using System.Collections;
	
	public class EA_ParabolicPath : EnemyAction {

		public GenerateRandomPositionNearPlayerAction positionGenerator;
		public Transform shadow;
		public float travelSpeed = 1f;
		public float pathHeight = 2f;

		private Vector3 targetPos;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished) {
			base.Init(e, onActionFinished);
		}

		public override void Execute() {
			print ("Parabolic path");
			targetPos = positionGenerator.GetGeneratedPosition();
			StartCoroutine(UpdateState());
		}

		private IEnumerator UpdateState() {
			// Save the shadow's local position (so we can reset it at the end of the parabolic path)
			Vector3 shadowOffset = shadow.localPosition;
			Vector3 startPos = e.transform.parent.position;
			e.body.Move(targetPos - startPos);
			e.invincible = true;
			float t = 0;
			while (t < 1) {
				Vector3 verticalOffset = new Vector3(0, (-Mathf.Pow(2 * t - 1, 2) + 1f) * pathHeight);						// Parabola!
				Vector3 position = Vector3.Lerp(startPos, targetPos, t) + verticalOffset;		// Object position with "elevation
				shadow.position = position - verticalOffset;
				e.transform.parent.position = position;
				t += Time.deltaTime * travelSpeed;
				yield return null;
			}
			e.invincible = false;
			shadow.localPosition = shadowOffset;
			e.transform.parent.position = targetPos;

			if (onActionFinished != null)
				onActionFinished();
		}

		public override void Interrupt() {}
	}
}