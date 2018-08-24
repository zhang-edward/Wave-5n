using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreReport : MonoBehaviour {

	public struct ScoreReportData
	{
		// public int enemiesDefeated, wavesSurvived, maxCombo;
		public int money, moneyEarned, souls, soulsEarned;
		public ScoreReportData (int money, int moneyEarned, int souls, int soulsEarned)
		{
			// this.enemiesDefeated = enemiesDefeated;
			// this.wavesSurvived = wavesSurvived;
			// this.maxCombo = maxCombo;
			this.money = money;
			this.moneyEarned = moneyEarned;
			this.souls = souls;
			this.soulsEarned = soulsEarned;
		}
	}

	//public IncrementingText enemiesDefeated, wavesSurvived, maxCombo;
	public IncrementingText moneyText, moneyEarned, soulsText, soulsEarned;

	public void ReportScore(ScoreReportData data)
	{
		moneyText.transform.parent.parent.GetComponent<Animator>().CrossFade("Pop", 0f);
		moneyText.DisplayNumber (data.money + data.moneyEarned); 
		moneyEarned.DisplayNumber (0);
		soulsText.DisplayNumber(data.souls + data.soulsEarned);
		soulsEarned.DisplayNumber(0);
		//StartCoroutine (ReportScoreTimed (data));
	}

	public void UpdateMoney(int money) {
		moneyText.transform.parent.parent.GetComponent<Animator>().CrossFade("Pop", 0f);
		moneyText.DisplayNumber(money);
	}

	public bool DoneUpdating() {
		return moneyText.doneUpdating && moneyEarned.doneUpdating && soulsText.doneUpdating && soulsEarned.doneUpdating;
	}

	// private IEnumerator ReportScoreTimed(ScoreReportData data)
	// {
	// 	// yield return new WaitForSeconds(.0f);
	// 	// enemiesDefeated.transform.parent.GetComponent<Animator>().CrossFade("Pop", 0f);
	// 	// enemiesDefeated.DisplayNumber (data.enemiesDefeated);
	// 	// while (!enemiesDefeated.doneUpdating)
	// 	// 	yield return null;
	// 	// yield return new WaitForSeconds(0.5f);

	// 	// maxCombo.transform.parent.GetComponent<Animator>().CrossFade("Pop", 0f);
	// 	// maxCombo.DisplayNumber (data.maxCombo);
	// 	// while (!maxCombo.doneUpdating)
	// 	// 	yield return null;
	// 	// yield return new WaitForSeconds(0.5f);

	// 	// wavesSurvived.transform.parent.GetComponent<Animator>().CrossFade("Pop", 0f);
	// 	// wavesSurvived.DisplayNumber(data.wavesSurvived);
	// 	// while (!wavesSurvived.doneUpdating)
	// 	// 	yield return null;
	// 	// yield return new WaitForSeconds(0.5f);


	// }
}
