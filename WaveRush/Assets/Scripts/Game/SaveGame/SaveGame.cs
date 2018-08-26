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
	public List<Pawn> availableHeroes;			// Heroes available for hire at the PawnShop
	/** Unlocked values: Increments of 3 per type, where #0-2 = knight t1, t2, t3, #3-5 = pyro t1, t2, t3, etc. */
	public bool[] unlockedHeroes;				// Types of heroes potentially available for hire
	public string[] unlockedSkins;				// Types of skins available to be worn by a hero

	[JsonConstructor]
	public SaveGame(Wallet wallet,
					PawnWallet pawnWallet,
					Dictionary<string, int> saveDict,
					bool[] unlockedHeroes,
					string[] unlockedSkins,
					List<Pawn> availableHeroes) {
		this.wallet = wallet;
		this.pawnWallet = pawnWallet;
		this.saveDict = saveDict;
		this.availableHeroes = availableHeroes;

		// Special initialization for array types
		int numHeroTypes = Enum.GetValues(typeof(HeroType)).Length * 3;
		this.unlockedHeroes = new bool  [numHeroTypes];
		this.unlockedSkins  = new string[numHeroTypes];
		if (unlockedHeroes != null) unlockedHeroes.CopyTo(this.unlockedHeroes, 0);
		if (unlockedSkins  != null) unlockedSkins .CopyTo(this.unlockedSkins, 0);
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
		unlockedSkins = new string[numHeroTypes];
		unlockedHeroes[0] = true;
	}
}