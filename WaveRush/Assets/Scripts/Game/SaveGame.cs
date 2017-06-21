using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	public const int INITIAL_PAWN_CAPACITY = 20;

	public HeroSaveData[] heroData;
	[Serializable]
	public class HeroSaveData 
	{
		public HeroType hero;
		public bool unlocked;
	}

	public int pawnCapacity { private get; set; }				// the total amount of pawns that the player can possess at one time
	public int numPawns;										// how many pawns the player has
	public Pawn[] pawns;                                        // the master list of the different pawns the player possesses

	public Dictionary<string, int> unlockedStages;      // key = name of the stage seriesression, value = number of stages in that seriesression that have been unlocked


	public Dictionary<HeroType, ScoreManager.Score> highScores;
	public Wallet wallet;

	public SaveGame()
	{
		unlockedStages = new Dictionary<string, int>();
		unlockedStages.Add("Gob Forest", 2);

		pawnCapacity = INITIAL_PAWN_CAPACITY;
		pawns = new Pawn[pawnCapacity];

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

	public bool AddPawn(Pawn pawn)
	{
		for (int i = 0; i < pawns.Length; i ++)
		{
			if (pawns[i] == null)
			{
				pawns[i] = pawn;
				pawn.SetID(i);
				Debug.Log(pawn);
				return true;
			}
		}
		return false;
	}

	public bool IsStageUnlocked(string stageName)
	{
		if (unlockedStages.ContainsKey(stageName))
		{
			if (unlockedStages[stageName] > 0)
				return true;
			else
				return false;
		}
		else
		{
			unlockedStages.Add(stageName, 0);
			return false;
		}
	}
}