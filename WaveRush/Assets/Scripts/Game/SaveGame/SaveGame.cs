using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class SaveGame
{
	/** Keys */
	public const string TUTORIAL_COMPLETE_KEY = "TutorialComplete";
	public const string LATEST_UNLOCKED_SERIES_INDEX_KEY = "SeriesIndex";
	public const string LATEST_UNLOCKED_STAGE_INDEX_KEY = "StageIndex";

	public Wallet 	wallet;
	public PawnWallet 	pawnWallet;
	/** Miscellaneous saved values */
	public Dictionary<string, int> saveDict;    // Dictionary containing integers
	public bool[] unlockedHeroes;				// Types of heroes potentially available for hire
												// Increments of 3 per type, where #0-2 = knight t1, t2, t3, #3-5 = pyro t1, t2, t3, etc.
	public List<Pawn> availableHeroes;		// Heroes available for hire

	[JsonConstructor]
	public SaveGame(Wallet wallet, 
					PawnWallet pawnWallet, 
					Dictionary<string, int> saveDict, 
					bool[] unlockedHeroes,
					List<Pawn> availableHeroes) {
		this.wallet = wallet;
		this.pawnWallet = pawnWallet;
		this.saveDict = saveDict;
		this.unlockedHeroes = unlockedHeroes;
		this.availableHeroes = availableHeroes;
	}

	public SaveGame() {
		// Initialize variables
		saveDict = new Dictionary<string, int>();

		saveDict[LATEST_UNLOCKED_SERIES_INDEX_KEY] = 0;
		saveDict[LATEST_UNLOCKED_STAGE_INDEX_KEY]  = 0;

		// hasPlayerViewedDict = new Dictionary<string, bool>();
		wallet = new Wallet();
		pawnWallet = new PawnWallet();

		// Get hero types
		int numHeroTypes = Enum.GetValues(typeof(HeroType)).Length * 3;
		unlockedHeroes = new bool[numHeroTypes];
		unlockedHeroes[0] = true;
	}
}