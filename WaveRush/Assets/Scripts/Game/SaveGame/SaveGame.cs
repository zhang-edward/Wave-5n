using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	//	public const int INITIAL_PAWN_CAPACITY = 20;
	public const string TUTORIAL_COMPLETE_KEY = "TutorialComplete";
	public const string LATEST_UNLOCKED_SERIES_INDEX_KEY = "SeriesIndex";
	public const string LATEST_UNLOCKED_STAGE_INDEX_KEY = "StageIndex";

	public Dictionary<HeroType, ScoreManager.Score> highScores;
	public Wallet wallet;
	public PawnWallet pawnWallet;

	//private Dictionary<string, float> timers; // list of various timers in the game, such as daily rewards and quests
	public Dictionary<string, int> saveDict;    // Dictionary containing any all data
	public Dictionary<string, bool> hasPlayerViewedDict { get; private set; }
	public bool[] unlockedHeroes;				// Heroes potentially available for hire
	public List<Pawn> availableHeroes;			// Heroes available for hire

	public SaveGame()
	{
		// Initialize variables
		saveDict = new Dictionary<string, int>();

		saveDict[LATEST_UNLOCKED_SERIES_INDEX_KEY] = 0;
		saveDict[LATEST_UNLOCKED_STAGE_INDEX_KEY]  = 0;

		hasPlayerViewedDict = new Dictionary<string, bool>();
		wallet = new Wallet();
		pawnWallet = new PawnWallet();

		// Get hero types
		int numHeroTypes = Enum.GetValues(typeof(HeroType)).Length;
		unlockedHeroes = new bool[numHeroTypes];
		unlockedHeroes[0] = true;

		// high scores are all 0 by default
		ClearHighScores();
	}

	public void ClearHighScores()
	{
		highScores = new Dictionary<HeroType, ScoreManager.Score>();
	}


	//public float GetSavedTimer(string key)
	//{
	//	if (timers.ContainsKey(key))
	//		return timers[key];
	//	else
	//		return -1;
	//}

	//public void SetSavedTimer(string key, float time)
	//{
	//	if (timers.ContainsKey(key))
	//		timers[key] = time;
	//	else
	//		timers.Add(key, time);
	//}
}