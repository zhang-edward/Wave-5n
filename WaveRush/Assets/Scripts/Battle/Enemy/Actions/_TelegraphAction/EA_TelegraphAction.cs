using UnityEngine;
using System.Collections;
namespace EnemyActions
{
	public abstract class EA_TelegraphAction : EnemyAction {

		[Header("Telegraph Properties")]
		public SpriteRenderer telegraphSprite;
		[Tooltip("The amount of time the circle is visible before the telegraph")]
		public float warningTime;
		public Color defaultColor = new Color(1, 1, 1, 0.5f);
		public Color flashColor = new Color(1, 0, 0, 0.5f);


		public override void Execute()
		{
			telegraphSprite.color = defaultColor;
			telegraphSprite.gameObject.SetActive(true);
			SpawnInTelegrapher();
			StartCoroutine(ExecuteRoutine());
		}

		protected abstract void SpawnInTelegrapher();

		private IEnumerator ExecuteRoutine()
		{
			yield return new WaitForSeconds(warningTime);
			// Flash red before telegraph
			bool isColor = false;
			for (int i = 0; i < 6; i++)
			{
				telegraphSprite.color = isColor ? defaultColor : flashColor;
				isColor = !isColor;
				yield return new WaitForSeconds(0.07f);
			}
			UnityEngine.Assertions.Assert.IsTrue(telegraphSprite.color == defaultColor);
			telegraphSprite.gameObject.SetActive(false);

			// Done
			if (onActionFinished != null)
				onActionFinished();
		}
	}
}