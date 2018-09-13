using UnityEngine;
using System.Collections;

namespace EnemyActions {
	
	public class EA_TelegraphCircle : EnemyAction {
		
		public SpriteRenderer telegraphSprite;
		public float size;				// Size of the circle
		public float warningTime;		// The amount of time the circle is visible before the telegraph
		public Color circleColor = new Color(1, 1, 1, 0.5f);
		public Color flashColor = new Color(1, 0, 0, 0.5f);


		public override void Execute() {
			StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine() {
			telegraphSprite.color = circleColor;
			telegraphSprite.gameObject.SetActive(true);

			// Spawn in telegrapher (spawn in for half-warning time, remain still for half-warning time)
			telegraphSprite.transform.localScale = Vector3.one * size * 1.3f;
			Vector3 endScale = Vector3.one * size;
			float halfWarningTime = warningTime / 2f;
			float t = 0;
			while (t < 1) {
				telegraphSprite.transform.localScale = Vector3.Lerp(telegraphSprite.transform.localScale, endScale, t);
				t += Time.deltaTime / halfWarningTime;
				yield return null;
			}
			telegraphSprite.transform.localScale = endScale;
			yield return new WaitForSeconds (halfWarningTime);

			// Flash red before telegraph
			bool isColor = false;
			for (int i = 0; i < 6; i ++) {
				telegraphSprite.color = isColor ? circleColor : flashColor;
				isColor = !isColor;
				yield return new WaitForSeconds(0.07f);
			}
			UnityEngine.Assertions.Assert.IsTrue(telegraphSprite.color == circleColor);

			telegraphSprite.gameObject.SetActive(false);

			// Done
			if (onActionFinished != null)
				onActionFinished();
		}

		public override void Interrupt() {
			StopAllCoroutines();
			telegraphSprite.gameObject.SetActive(false);
		}

		void OnDrawGizmosSelected() {
			Gizmos.DrawWireSphere(transform.position, size);
		}
	}
}