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

	public int pawnCapacity { private get; set; }	// the total amount of pawns that the player can possess at one time
	public int numPawns;							// how many pawns the player has
	public Pawn[] pawns { get; private set; }		// the master list of the different pawns the player possesses
	public Pawn[] extraPawns { get; private set; }	// a list of extra pawns acquired - must be empty to enter battle

	public int latestUnlockedSeriesIndex;   // the number of series unlocked in the regular collection (main storyline)
	public int latestUnlockedStageIndex;	// the number of stages unlocked in the current latest series

	public Dictionary<HeroType, ScoreManager.Score> highScores;
	public Wallet wallet;

	private Dictionary<string, float> timers;   // list of various timers in the game, such as daily rewards and quests
	public int numDailyHeroRewards;

	public SaveGame()
	{
		pawnCapacity = INITIAL_PAWN_CAPACITY;
		pawns = new Pawn[pawnCapacity];
		extraPawns = new Pawn[10];

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

	// ========== 
	// Pawns
	// ==========
	public bool AddPawn(Pawn pawn, bool overflow = true, float unlockTime = 0)
	{
		for (int i = 0; i < pawns.Length; i++)
		{
			if (pawns[i] == null)
			{
				pawn.SetID(i);
				pawn.unlockTime = unlockTime;
				if (unlockTime > 0)
				{
					GameManager.instance.timerCounter.SetTimer(pawn.GetTimerID(), unlockTime);
				}
				pawns[i] = pawn;
				Debug.Log("New Pawn:" + pawn + " with unlock time:" + unlockTime);
				return true;
			}
		}
		if (overflow)
		{
			AddOverflowPawn(pawn, unlockTime);
			return true;
		}
		return false;
	}

	// Add a pawn when pawn capacity has been reached
	private void AddOverflowPawn(Pawn pawn, float unlockTime = 0)
	{
		int debugCounter = 0;
		while (debugCounter < 10)		// if we are trying to add 1024+ extra pawns (10 resizings = 2^10 = 1024)
		{
			for (int i = 0; i < extraPawns.Length; i++)
			{
				if (extraPawns[i] == null)
				{
					pawn.SetID(i + pawnCapacity);
					pawn.unlockTime = unlockTime;
					extraPawns[i] = pawn;
					Debug.Log("New Pawn (Overflow):" + pawn + " with unlock time:" + unlockTime);
					return;
				}
			}
			// Resize extraPawnsList, if needed
			Pawn[] newExtraPawns = new Pawn[extraPawns.Length * 2];
			for (int i = 0; i < extraPawns.Length; i ++)
				newExtraPawns[i] = extraPawns[i];
			extraPawns = newExtraPawns;

			debugCounter++;
		}
		Debug.LogError("1000+ tries to fit a new Pawn into the extraPawns list!" +
					   "extraPawns list has " + extraPawns.Length + " pawns");
	}

	public Pawn GetPawn(int id)
	{
		if (id < pawnCapacity)
			return pawns[id];
		else
			return extraPawns[id - pawnCapacity];
	}

	public bool RemovePawn(int id)
	{
		if (id < pawnCapacity)
		{
			if (pawns[id] != null)
			{
				Debug.Log("Removed Pawn:" + pawns[id]);
				pawns[id] = null;
				if (HasExtraPawns())
				{
					int i = 0;
					while (i < extraPawns.Length && extraPawns[i] == null)
					{
						i++;
					}
					Debug.Log("Extra pawn filled slot" + i + ":" + extraPawns[i]);
					AddPawn(extraPawns[i]);
					extraPawns[i] = null;
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			int extraId = id - pawnCapacity;
			if (extraPawns[extraId] != null)
			{
				Debug.Log("Removed Pawn (Extra):" + extraPawns[extraId]);
				extraPawns[extraId] = null;
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public bool HasExtraPawns()
	{
		for (int i = 0; i < extraPawns.Length; i++)
		{
			if (extraPawns[i] != null)
				return true;
		}
		return false;
	}
}