using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	//	public const int INITIAL_PAWN_CAPACITY = 20;
	public const string TUTORIAL_COMPLETE_KEY = "TutorialComplete";

	public HeroSaveData[] heroData;
	[Serializable]
	public class HeroSaveData 
	{
		public HeroType hero;
		public bool unlocked;
	}

	public PawnWallet pawnWallet;
//	public int pawnCapacity { private get; set; }	// the total amount of pawns that the player can possess at one time
//	public int numPawns;							// how many pawns the player has
//	public Pawn[] pawns { get; private set; }		// the master list of the different pawns the player possesses
//	public Pawn[] extraPawns { get; private set; }	// a list of extra pawns acquired - must be empty to enter battle

	public int latestUnlockedSeriesIndex;   // the number of series unlocked in the regular collection (main storyline)
	public int latestUnlockedStageIndex;	// the number of stages unlocked in the current latest series

	public Dictionary<HeroType, ScoreManager.Score> highScores;
	public Wallet wallet;

	private Dictionary<string, float> timers;   // list of various timers in the game, such as daily rewards and quests
	public Dictionary<string, int> saveDict;	// Dictionary containing any misc. data
	public int numDailyHeroRewards;

	public SaveGame()
	{
		//		pawnCapacity = INITIAL_PAWN_CAPACITY;
		//		pawns = new Pawn[pawnCapacity];
		//		extraPawns = new Pawn[10];
		pawnWallet = new PawnWallet();
		int numHeroTypes = Enum.GetNames(typeof(HeroType)).Length;
		heroData = new HeroSaveData[numHeroTypes];
		// default all heroes locked but the first hero (the knight)
		for (int i = 0; i < numHeroTypes; i ++)
		{
			HeroType type = (HeroType)Enum.GetValues(typeof(HeroType)).GetValue(i);
			heroData[i] = new HeroSaveData();
			heroData[i].hero = type;
		}
		heroData[0].unlocked = true;
		// high scores are all 0 by default
		ClearHighScores();
		// wallet money = 0 by default
		timers = new Dictionary<string, float>();
		wallet = new Wallet();
	}

	public void ClearHighScores()
	{
		highScores = new Dictionary<HeroType, ScoreManager.Score>();
	}

	public HeroSaveData GetHeroData(HeroType hero)
	{
		foreach (HeroSaveData data in heroData)
		{
			if (data.hero == hero)
				return data;
		}
		return null;
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