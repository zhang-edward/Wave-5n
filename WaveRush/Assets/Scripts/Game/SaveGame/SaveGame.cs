using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
	/** Keys */
	public const string TUTORIAL_COMPLETE_KEY = "TutorialComplete";
	public const string LATEST_UNLOCKED_SERIES_INDEX_KEY = "SeriesIndex";
	public const string LATEST_UNLOCKED_STAGE_INDEX_KEY = "StageIndex";

	/** Wallet fields */	
	private int money;
	private int souls;
	public Wallet wallet;

	/** PawnWallet fields */
	private int pawnCapacity;
	private string[] pawns;
	public PawnWallet pawnWallet;

	/** Miscellaneous saved values */
	public Dictionary<string, int> saveDict;    // Dictionary containing integers
	public Dictionary<string, bool> hasPlayerViewedDict { get; private set; }

	public bool[] unlockedHeroes;				// Heroes potentially available for hire
	public List<string> availableHeroes;			// Heroes available for hire

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
	}
}