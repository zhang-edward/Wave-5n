using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreReport : MonoBehaviour {

	public IncrementingText enemiesDefeated, wavesSurvived, maxCombo;
	public IncrementingText moneyText, moneyEarned;

	public void ReportScore(int enemiesDefeatedNum, int wavesSurvivedNum, int maxComboNum)
	{
		enemiesDefeated.ReportScore (enemiesDefeatedNum);
		wavesSurvived.ReportScore (wavesSurvivedNum);
		maxCombo.ReportScore (maxComboNum);

		int currentMoney = GameManager.instance.wallet.money;
		int moneyEarnedNum = GameManager.instance.wallet.moneyEarned;

		moneyText.ReportScore (currentMoney + moneyEarnedNum); 
		moneyEarned.ReportScore (0);

		GameManager.instance.wallet.MergeEarnedMoney ();
		GameManager.instance.UpdateScores (enemiesDefeatedNum, wavesSurvivedNum, maxComboNum);
	}
}
