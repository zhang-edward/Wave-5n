using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreReport : MonoBehaviour {

	public IncrementingText enemiesDefeated, wavesSurvived, maxCombo;
	public IncrementingText moneyText, moneyEarned;

	public void ReportScore(int enemiesDefeatedNum, int wavesSurvivedNum, int maxComboNum)
	{
		StartCoroutine (ReportScoreTimed (enemiesDefeatedNum, wavesSurvivedNum, maxComboNum));
	}

	private IEnumerator ReportScoreTimed(int enemiesDefeatedNum, int wavesSurvivedNum, int maxComboNum)
	{
		int currentMoney = GameManager.instance.wallet.money;
		int moneyEarnedNum = GameManager.instance.wallet.moneyEarned;

		GameManager.instance.wallet.MergeEarnedMoney ();
		GameManager.instance.UpdateScores (enemiesDefeatedNum, wavesSurvivedNum, maxComboNum);

		enemiesDefeated.ReportScore (enemiesDefeatedNum);
		while (!enemiesDefeated.doneUpdating)
			yield return null;
		
		wavesSurvived.ReportScore (wavesSurvivedNum);
		while (!wavesSurvived.doneUpdating)
			yield return null;
		
		maxCombo.ReportScore (maxComboNum);
		while (!maxCombo.doneUpdating)
			yield return null;

		moneyText.ReportScore (currentMoney + moneyEarnedNum); 
		moneyEarned.ReportScore (0);
	}
}
