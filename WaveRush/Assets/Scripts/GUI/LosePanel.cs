using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LosePanel : MonoBehaviour
{
	public Button proceedButton;
	public TMP_Text stageNameText;

	public ScrollViewSnap scrollView;
	public ScoreReport scoreReport;
	public HeroesRescuedMenu heroesRescuedMenu;

	private List<Pawn> acquiredPawns;
	private ScoreReport.ScoreReportData scoreReportData;

	public void Init(ScoreReport.ScoreReportData scoreReportData, List<Pawn> acquiredPawns, string stageName) {
		this.scoreReportData = scoreReportData;
		this.acquiredPawns = acquiredPawns;

		stageNameText.text = stageName;
		InitScoreReportView();
	}

	void InitHeroesRescuedView() {
		scrollView.ScrollRight();
		heroesRescuedMenu.Init(acquiredPawns);
		proceedButton.onClick.RemoveAllListeners();
		proceedButton.onClick.AddListener(GoToMenuScene);
	}

	private void InitScoreReportView()
	{
		StartCoroutine(InitScoreReportViewRoutine());
	}

	private IEnumerator InitScoreReportViewRoutine()
	{
		scoreReport.moneyText.text.text = scoreReportData.money.ToString();
		scoreReport.moneyEarned.text.text = scoreReportData.moneyEarned.ToString();
		scoreReport.soulsText.text.text = scoreReportData.souls.ToString();
		scoreReport.soulsEarned.text.text = scoreReportData.soulsEarned.ToString();		
		yield return new WaitForSeconds(1.5f);
		scoreReport.ReportScore(scoreReportData);
		// If we don't have any heroes acquired
		if (acquiredPawns.Count <= 0) {
			proceedButton.onClick.AddListener(GoToMenuScene);
		}
		// Otherwise, show the heroes rescued menu
		else {
			proceedButton.onClick.AddListener(InitHeroesRescuedView);
		}
	}

	private void GoToMenuScene()
	{
		GameManager.instance.GoToScene("MainMenu");
	}
}
