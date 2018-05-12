using UnityEngine;
using System;
using System.Collections.Generic;

public class PawnShop {

	public Pawn[] Pawns { get; private set; }   // All potentially available pawns
	public bool[] Unlocked { get; private set; }   // Whether or not the pawn at index i is available

	public List<Pawn> AvailablePawns { get; private set; }

	public SaveModifier.PawnStateUpdate OnPawnListUpdated;

	/// <summary>
	/// Initializes a new instance of the <see cref="PawnShop"/> class.
	/// </summary>
	public PawnShop() {
		AvailablePawns = new List<Pawn>();
		// Get hero types
		HeroType[] types = (HeroType[])Enum.GetValues(typeof(HeroType));
		int numHeroTypes = types.Length - 1;

		// Initialize the available pawns (t1, t2, t3 for each hero type, not counting the null hero)
		Pawns = new Pawn[(numHeroTypes) * 3];
		for (int i = 0; i < numHeroTypes; i++) {
			HeroType type = types[i + 1];
			Pawns[i * 3] 	 = new Pawn(type, HeroTier.tier1);
			Pawns[i * 3 + 1] = new Pawn(type, HeroTier.tier2);
			Pawns[i * 3 + 2] = new Pawn(type, HeroTier.tier3);
		}
		Unlocked = new bool[Pawns.Length];
	}

	/// <summary>
	/// Called when the save game is loaded. Initializes <see cref="Unlocked"/> array
	/// </summary>
	/// <param name="save">Save.</param>
	public void OnSaveGameLoaded(SaveModifier save) {
		int latestSeries = save.LatestSeriesIndex;
		int latestStage  = save.LatestStageIndex;
		for (int i = 0; i < Pawns.Length; i++) {
			Pawn pawn = Pawns[i];
			HeroData data = DataManager.GetHeroData(pawn.type);
			if (latestSeries >= data.unlockSeries[(int)pawn.tier] &&
				latestStage >= data.unlockStage[(int)pawn.tier]) {
				Unlocked[i] = true;
				AvailablePawns.Add(pawn);
			}
		}
	}
}
