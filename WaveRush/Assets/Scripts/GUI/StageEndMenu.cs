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
	[Header("Bonus")]
	public GameObject bonusMenu;
	public IncrementingText bonusMoneyText;
	public Button rewardedAdButton;
	public AdsManager adsManager;

	/** Data */
	private ScoreReport.ScoreReportData scoreReportData;
	private HeroExpMenu.HeroExpMenuData[] heroExpMenuData;
	int bonusMoney;

	public void Init(ScoreReport.ScoreReportData scoreReportData, 
	                 HeroExpMenu.HeroExpMenuData[] heroExpMenuData,
					 int bonusMoney,
	                 string stageName) {
		this.scoreReportData = scoreReportData;
		this.heroExpMenuData = heroExpMenuData;
		this.bonusMoney = bonusMoney;

		stageNameText.text = stageName;
		StartCoroutine(StageEndMenuRoutine());

		scoreReport.moneyText.text.text 	= scoreReportData.money.ToString();
		scoreReport.moneyEarned.text.text	= " +" + scoreReportData.moneyEarned.ToString();
		scoreReport.soulsText.text.text 	= scoreReportData.souls.ToString();
		scoreReport.soulsEarned.text.text 	= " +" + scoreReportData.soulsEarned.ToString();	
		bonusMoneyText.text.text			= bonusMoney.ToString();

		bonusMenu.SetActive(false);
		proceedButton.gameObject.SetActive(false);

		adsManager.OnRewardedVideoAdShown += OnRewardClaimed;
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

	private void InitScoreReportView() {
		StartCoroutine(InitScoreReportViewRoutine());
	}

	private IEnumerator InitScoreReportViewRoutine()
	{
		yield return new WaitForSeconds(1.0f);
		scoreReport.ReportScore(scoreReportData);
		while (!scoreReport.DoneUpdating())
			yield return null;
		
		yield return new WaitForSeconds(1.0f);
		bonusMenu.SetActive(true);
		
		yield return new WaitForSeconds(1.0f);
		proceedButton.gameObject.SetActive(true);
		// If we don't have any heroes acquired
		//if (acquiredPawns.Count <= 0) {
		//	proceedButton.onClick.AddListener(GoToMenuScene);
		//}
		//// Otherwise, show the heroes rescued menu
		//else {
			//proceedButton.onClick.AddListener(InitHeroExpMenu);
		//}
	}

	private void OnRewardClaimed(int id) {
		if (id != 0)
			return;
		GameManager.instance.save.AddMoney(bonusMoney);
		bonusMoneyText.DisplayNumber(0);
		scoreReport.UpdateMoney(bonusMoney + scoreReportData.moneyEarned + scoreReportData.money);
		rewardedAdButton.interactable = false;
	}

	private void GoToMenuScene() {
		GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
	}
}
