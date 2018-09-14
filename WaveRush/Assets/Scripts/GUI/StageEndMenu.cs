using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageEndMenu : MonoBehaviour
{

	private const int AD_REWARD_MONEY = 0;

	[Header("Menu Components")]
	public TMP_Text stageNameText;
	public ScoreReport scoreReport;
	public UIAnimatorControl scoreReportAnimator;
	public HeroExpMenu heroesExpMenu;
	public UIAnimatorControl heroesExpMenuAnimator;
	public Button proceedButton;
	[Header("Bonus")]
	public GameObject bonusMenu;
	public IncrementingText bonusMoneyText;
	public Button rewardedAdButton;
	public AdsManager adsManager;

	[Header("Audio")]
	public AudioClip bonusMenuClip;
	public AudioClip proceedButtonClip;

	/** Data */
	private ScoreReport.ScoreReportData scoreReportData;
	private HeroExpMenu.HeroExpMenuData[] heroExpMenuData;

	public void Init(ScoreReport.ScoreReportData scoreReportData,
					 HeroExpMenu.HeroExpMenuData[] heroExpMenuData,
					 string stageName)
	{
		this.scoreReportData = scoreReportData;
		this.heroExpMenuData = heroExpMenuData;

		stageNameText.text = stageName;
		StartCoroutine(StageEndMenuRoutine());

		bonusMenu.SetActive(false);
		proceedButton.gameObject.SetActive(false);
		adsManager.OnRewardedVideoAdShown += OnRewardClaimed;
	}

	public IEnumerator StageEndMenuRoutine()
	{
		heroesExpMenu.Init(heroExpMenuData);
		while (!heroesExpMenu.doneAnimating)
			yield return null;
		while (!Input.GetMouseButtonDown(0))
			yield return null;
		heroesExpMenuAnimator.AnimateOut();
		StartCoroutine(InitScoreReportViewRoutine());
	}

	public void ProceedButtonPressed()
	{
		scoreReportAnimator.AnimateOut();
		proceedButton.interactable = false;
		Invoke("GoToMenuScene", 1.0f);
	}

	private IEnumerator InitScoreReportViewRoutine()
	{
		scoreReportAnimator.gameObject.SetActive(true);

		scoreReport.Init(scoreReportData);
		yield return new WaitForSeconds(1.0f);
		scoreReport.ReportScore();
		while (!scoreReport.DoneUpdating())
			yield return null;

		yield return new WaitForSeconds(1.0f);
		bonusMenu.SetActive(true);
		bonusMoneyText.text.text = scoreReportData.bonusMoney.ToString();
		SoundManager.instance.RandomizeSFX(bonusMenuClip);

		yield return new WaitForSeconds(1.0f);
		proceedButton.gameObject.SetActive(true);
		SoundManager.instance.RandomizeSFX(proceedButtonClip);
	}

	private void OnRewardClaimed(int id)
	{
		switch (id)
		{
			case AD_REWARD_MONEY:
				GameManager.instance.save.AddMoney(scoreReportData.bonusMoney);
				bonusMoneyText.DisplayNumber(0);
				scoreReport.UpdateMoney(scoreReportData.bonusMoney + scoreReportData.moneyEarned + scoreReportData.money);
				rewardedAdButton.interactable = false;
				break;
			default:
				Debug.LogError("AdManager returned an invalid identifier!");
				break;
		}
	}

	private void GoToMenuScene()
	{
		GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
	}
}
