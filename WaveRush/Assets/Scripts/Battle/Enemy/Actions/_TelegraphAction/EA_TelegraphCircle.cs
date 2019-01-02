using UnityEngine;
using System.Collections;

namespace EnemyActions {
	
	public class EA_TelegraphCircle : EA_TelegraphAction {
		
		public float size;

		protected override void SpawnInTelegrapher() {
			StartCoroutine(SpawnInTelegrapherRoutine());
		}

		private IEnumerator SpawnInTelegrapherRoutine() {
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