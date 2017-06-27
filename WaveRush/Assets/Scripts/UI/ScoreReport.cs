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

		enemiesDefeated.DisplayNumber (enemiesDefeatedNum);
		while (!enemiesDefeated.doneUpdating)
			yield return null;
		
		wavesSurvived.DisplayNumber (wavesSurvivedNum);
		while (!wavesSurvived.doneUpdating)
			yield return null;
		
		maxCombo.DisplayNumber (maxComboNum);
		while (!maxCombo.doneUpdating)
			yield return null;

		moneyText.DisplayNumber (currentMoney + moneyEarnedNum); 
		moneyEarned.DisplayNumber (0);
	}
}
