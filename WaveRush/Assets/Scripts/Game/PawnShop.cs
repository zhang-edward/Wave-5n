using UnityEngine;
using System.Collections;
using System;

public class PawnShop {
	
	private Pawn[] pawns;		// All potentially available pawns
	private bool[] unlocked;	// Whether or not the pawn at index i is available

	/// <summary>
	/// Initializes a new instance of the <see cref="PawnShop"/> class.
	/// </summary>
	public PawnShop() {
		// Get hero types
		HeroType[] types = (HeroType[])Enum.GetValues(typeof(HeroType));
		int numHeroTypes = types.Length;

		// Initialize the available pawns (t1, t2, t3 for each hero type, not counting the null hero)
		pawns = new Pawn[(numHeroTypes - 1) * 3];
		for (int i = 0; i < numHeroTypes; i++) {
			HeroType type = types[i];
			pawns[i] 	 = new Pawn(type, HeroTier.tier1);
			pawns[i + 1] = new Pawn(type, HeroTier.tier2);
			pawns[i + 2] = new Pawn(type, HeroTier.tier3);
		}
	}

	/// <summary>
	/// Called when the save game is loaded. Initializes <see cref="unlocked"/> array
	/// </summary>
	/// <param name="save">Save.</param>
	public void OnSaveGameLoaded(SaveModifier save) {
		int latestSeries = save.LatestSeriesIndex;
		int latestStage  = save.LatestStageIndex;
		for (int i = 0; i < pawns.Length; i ++) {
			Pawn pawn = pawns[i];
			HeroData data = DataManager.GetHeroData(pawn.type);
			if (latestSeries >= data.unlockSeries[(int)pawn.tier] &&
				latestStage >= data.unlockStage[(int)pawn.tier])
				unlocked[i] = true;
		}
	}
}
