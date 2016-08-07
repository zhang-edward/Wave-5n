using UnityEngine;
using System.Collections;

public class ScoreReport : MonoBehaviour {

	public ScoreReportNumber enemiesDefeated, wavesSurvived, maxCombo;

	public void ReportScore(int enemiesDefeatedNum, int wavesSurvivedNum)
	{
		enemiesDefeated.ReportScore (enemiesDefeatedNum);
		wavesSurvived.ReportScore (wavesSurvivedNum);
		//maxCombo.ReportScore (maxComboNum);

		GameManager.instance.UpdateScores (enemiesDefeatedNum, wavesSurvivedNum);
	}
}
