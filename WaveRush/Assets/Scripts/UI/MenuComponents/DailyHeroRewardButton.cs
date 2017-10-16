using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class DailyHeroRewardButton : MonoBehaviour
{
	private const string TIMER_KEY = "DailyHeroRewardButton";
	private const float REWARD_INTERVAL = 100f;
	private const int MAX_REWARDS = 3;

	[Header("Set from Inspector")]
	public DialogueView dialogueView;
	public HeroesRescuedMenu heroesRescuedMenu;
	[Header("Set in Prefab")]
	public TimerView timerView;
	public TMP_Text numRewardsText;
	public DialogueSet dialogueSuccess, dialogueFailure;

	private RealtimeTimerCounter timerCounter;
	private SaveGame saveGame;
	private float timeUntilNextReward;
	private int currentNumRewards;
	private Outline outline;

	private Button button;

	void Awake()
	{
		timerCounter = GameManager.instance.timerCounter;
		button = GetComponent<Button>();
		outline = GetComponent<Outline>();

		timerCounter = GameManager.instance.timerCounter;
		saveGame = GameManager.instance.saveGame;
		currentNumRewards = saveGame.numDailyHeroRewards;

		// If the timer has not been initialized yet (app is opened for the first time)
		// Since timerCounter is a singleton instance under GameManager, when this scene is reloaded later
		// after the app has been opened, the timer will already be initialized and therefore InitTimer() will
		// not be run.
		if (timerCounter.GetTimer(TIMER_KEY) == null)
			InitTimer();
		else if (timerCounter.GetTimer(TIMER_KEY).timer <= 0 && currentNumRewards < MAX_REWARDS)
			UpdateRewardsSinceLastLogin();
	}

	void Start()
	{
		button.onClick.AddListener(() => StartCoroutine(ButtonPressedRoutine()));
	}

	void OnEnable()
	{
		saveGame = GameManager.instance.saveGame;
		GameManager.instance.OnAppClosed += SaveTimer;
		GameManager.instance.OnTimersUpdated += UpdateRewardsSinceLastLogin;
	}


	void OnDisable()
	{
		GameManager.instance.OnAppClosed -= SaveTimer;
		GameManager.instance.OnTimersUpdated -= UpdateRewardsSinceLastLogin;
		saveGame.numDailyHeroRewards = currentNumRewards;
	}

	private IEnumerator ButtonPressedRoutine()
	{
		if (currentNumRewards > 0)
		{
			heroesRescuedMenu.Reset();
			dialogueView.Init(dialogueSuccess);
			while (dialogueView.dialoguePlaying)
				yield return null;

			List<Pawn> rewards = new List<Pawn>();
			for (int i = 0; i < currentNumRewards; i++)
			{
				Pawn p = PawnGenerator.GenerateCrystalDrop(1);
				saveGame.pawnWallet.AddPawn(p);
				rewards.Add(p);
			}
			heroesRescuedMenu.gameObject.SetActive(true);
			heroesRescuedMenu.Init(rewards);
			if (timerCounter.GetTimer(TIMER_KEY).timer <= 0)
			{
				timeUntilNextReward = REWARD_INTERVAL;
				ResetTimer();
			}
			currentNumRewards = 0;
		}
		else
		{
			dialogueView.Init(dialogueFailure);
		}
		yield return null;
	}

	void Update()
	{
		numRewardsText.text = currentNumRewards.ToString();
	}

	private void GetNewReward()
	{
		currentNumRewards++;
		saveGame.numDailyHeroRewards = currentNumRewards;
		timeUntilNextReward = REWARD_INTERVAL;
		if (currentNumRewards >= MAX_REWARDS)
		{
			currentNumRewards = MAX_REWARDS;
			return;
		}
		ResetTimer();
	}

	private void InitTimer()
	{
		print("Init timer");
		currentNumRewards = saveGame.numDailyHeroRewards;
		timeUntilNextReward = saveGame.GetSavedTimer(TIMER_KEY);
		if (timeUntilNextReward < 0 && currentNumRewards < MAX_REWARDS)
		{
			timeUntilNextReward = REWARD_INTERVAL;
		}
		ResetTimer();
	}

	private void SaveTimer()
	{
		//print("Save timer");
		timeUntilNextReward = timerCounter.GetTimer(TIMER_KEY).timer;
		saveGame.SetSavedTimer(TIMER_KEY, timeUntilNextReward);
		saveGame.numDailyHeroRewards = currentNumRewards;

	}

	private void UpdateRewardsSinceLastLogin()
	{
		float timerTime = timerCounter.GetTimer(TIMER_KEY).timer;
		//print("Time since last logged in: " + timerTime);
		if (timerTime > 0 || currentNumRewards >= MAX_REWARDS)
			return;
		// The number of rewards since the last login time is equal to the negative time divided by the reward interval
		// since that is the amount of time that has surpassed
		int numRewardsSinceLastLogin = Mathf.FloorToInt(Mathf.Abs(timerTime) / REWARD_INTERVAL) + 1;
		// Set the number of rewards to the specified cap, if it was surpassed
		currentNumRewards = Mathf.Min(currentNumRewards + numRewardsSinceLastLogin, MAX_REWARDS);
		// Save the data in the saveGame
		saveGame.numDailyHeroRewards = currentNumRewards;

		timeUntilNextReward = timerTime % REWARD_INTERVAL;
		// Reset the timer to the appropriate time
		if (currentNumRewards < MAX_REWARDS)
			timeUntilNextReward = REWARD_INTERVAL - Mathf.Abs(timerTime) % REWARD_INTERVAL;
		else
			timeUntilNextReward = 0;
		ResetTimer();
	}

	private void ResetTimer()
	{
		timerCounter.SetTimer(TIMER_KEY, timeUntilNextReward, GetNewReward);
		timerView.timer = timerCounter.GetTimer(TIMER_KEY);
	}
}
