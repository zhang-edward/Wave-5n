using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreReport : MonoBehaviour {

	public class ScoreReportData
	{
		public int enemiesDefeated, wavesSurvived, maxCombo;
		public int money, moneyEarned;
		public ScoreReportData (int enemiesDefeated, int wavesSurvived, int maxCombo, int money, int moneyEarned)
		{
			this.enemiesDefeated = enemiesDefeated;
			this.wavesSurvived = wavesSurvived;
			this.maxCombo = maxCombo;
			this.money = money;
			this.moneyEarned = moneyEarned;
		}
	}

	public IncrementingText enemiesDefeated, wavesSurvived, maxCombo;
	public IncrementingText moneyText, moneyEarned;

	public void ReportScore(ScoreReportData data)
	{
		StartCoroutine (ReportScoreTimed (data));
	}

	private IEnumerator ReportScoreTimed(ScoreReportData data)
	{
		enemiesDefeated.DisplayNumber (data.enemiesDefeated);
		while (!enemiesDefeated.doneUpdating)
			yield return null;
		
		maxCombo.DisplayNumber (data.maxCombo);
		while (!maxCombo.doneUpdating)
			yield return null;

		wavesSurvived.DisplayNumber(data.wavesSurvived);
		while (!wavesSurvived.doneUpdating)
			yield return null;

		moneyText.DisplayNumber (data.money + data.moneyEarned); 
		moneyEarned.DisplayNumber (0);
	}
}
