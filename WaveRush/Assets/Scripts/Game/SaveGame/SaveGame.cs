using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	//	public const int INITIAL_PAWN_CAPACITY = 20;
	public const string TUTORIAL_COMPLETE_KEY = "TutorialComplete";

	public Dictionary<string, bool> hasPlayerViewedDict { get; private set; }
	public PawnWallet pawnWallet;

	public int latestUnlockedSeriesIndex;   // the number of series unlocked in the regular collection (main storyline)
	public int latestUnlockedStageIndex;	// the number of stages unlocked in the current latest series

	public Dictionary<HeroType, ScoreManager.Score> highScores;
	public Wallet wallet;

	private Dictionary<string, float> timers;   // list of various timers in the game, such as daily rewards and quests
	public Dictionary<string, int> saveDict;	// Dictionary containing any misc. data
	public int numDailyHeroRewards;

	public SaveGame()
	{
		// Initialize variables
		pawnWallet = new PawnWallet();
		hasPlayerViewedDict = new Dictionary<string, bool>();
		timers = new Dictionary<string, float>();
		wallet = new Wallet();
		// high scores are all 0 by default
		ClearHighScores();
	}

	public void ClearHighScores()
	{
		highScores = new Dictionary<HeroType, ScoreManager.Score>();
	}

	public float GetSavedTimer(string key)
	{
		if (timers.ContainsKey(key))
			return timers[key];
		else
			return -1;
	}

	public void SetSavedTimer(string key, float time)
	{
		if (timers.ContainsKey(key))
			timers[key] = time;
		else
			timers.Add(key, time);
	}
}