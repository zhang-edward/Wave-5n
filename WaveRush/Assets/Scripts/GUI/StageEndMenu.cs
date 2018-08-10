using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageEndMenu : MonoBehaviour {
	
	/** UI */
	public Button proceedButton;
	public TMP_Text stageNameText;
	public ScoreReport scoreReport;
	public HeroExpMenu heroesExpMenu;

	/** Data */
	private ScoreReport.ScoreReportData scoreReportData;
	private HeroExpMenu.HeroExpMenuData[] heroExpMenuData;

	public void Init(ScoreReport.ScoreReportData scoreReportData, 
	                 HeroExpMenu.HeroExpMenuData[] heroExpMenuData, 
	                 string stageName) {
		this.scoreReportData = scoreReportData;
		this.heroExpMenuData = heroExpMenuData;

		stageNameText.text = stageName;
		StartCoroutine(StageEndMenuRoutine());

		scoreReport.moneyText.text.text 	= scoreReportData.money.ToString();
		scoreReport.moneyEarned.text.text	= scoreReportData.moneyEarned.ToString();
		scoreReport.soulsText.text.text 	= scoreReportData.souls.ToString();
		scoreReport.soulsEarned.text.text 	= scoreReportData.soulsEarned.ToString();		

	}

	public IEnumerator StageEndMenuRoutine() {
		InitHeroExpMenu();
		while (!heroesExpMenu.doneAnimating)
			yield return null;
		InitScoreReportView();
	}

	void InitHeroExpMenu() {
		// scrollView.ScrollRight();
		heroesExpMenu.Init(heroExpMenuData);
		//heroesRescuedMenu.Init(acquiredPawns);
		//proceedButton.onClick.RemoveAllListeners();
		proceedButton.onClick.AddListener(GoToMenuScene);
	}

	private void InitScoreReportView()
	{
		StartCoroutine(InitScoreReportViewRoutine());
	}

	private IEnumerator InitScoreReportViewRoutine()
	{
		yield return new WaitForSeconds(1f);
		scoreReport.ReportScore(scoreReportData);
		// If we don't have any heroes acquired
		//if (acquiredPawns.Count <= 0) {
		//	proceedButton.onClick.AddListener(GoToMenuScene);
		//}
		//// Otherwise, show the heroes rescued menu
		//else {
			//proceedButton.onClick.AddListener(InitHeroExpMenu);
		//}
	}

	private void GoToMenuScene()
	{
		GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
	}
}
