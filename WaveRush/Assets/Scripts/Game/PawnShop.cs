using UnityEngine;
using System.Collections.Generic;

public class PawnShop {
	private List<Pawn> pawnPool = new List<Pawn>();     // The list of pawns which AvailablePawns selects from
	private SaveModifier save;

	public List<Pawn> AvailablePawns { get; private set; }	// The available pawns in the pawn shop
	public SaveModifier.PawnStateUpdate OnPawnListUpdated;


	/// <summary>
	/// Initializes a new instance of the <see cref="PawnShop"/> class.
	/// </summary>
	public PawnShop(SaveModifier save) {
		this.save = save;
		AvailablePawns = new List<Pawn>();
	}

	public void OnSaveGameLoaded() {
		RefreshPawnPool();
		if (save.AvailableHeroes == null)
			RefreshAvailablePawns();
		else
			AvailablePawns = save.AvailableHeroes;
	}

	public void RefreshPawnPool() {
		bool[] unlockedHeroes = save.UnlockedHeroes;
		for (int i = 0; i < unlockedHeroes.Length; i++) {
			if (unlockedHeroes[i]) {
				HeroType type = (HeroType)(i / 3);
				HeroTier tier = (HeroTier)(i % 3);
				pawnPool.Add(new Pawn(type, tier));
			}
		}
	}

	public void RefreshAvailablePawns() {
		AvailablePawns.Clear();
		for (int i = 0; i < 5; i++) {
			// Randomly modify a pawn from the pawn pool, then add it to the available pawns
			Pawn pawn = RandomlyModify(pawnPool[Random.Range(0, pawnPool.Count)]);
			pawn.SetID(i);
			AvailablePawns.Add(pawn);
		}
		save.AvailableHeroes = AvailablePawns;
	}

	public void RemovePawn(Pawn pawn) {
		AvailablePawns.Remove(pawn);
	}

	private Pawn RandomlyModify(Pawn pawn) {
		Pawn ans = new Pawn(pawn);
		/** Random level up */
		for (int i = 5; i >= 1; i ++) {
			if (Random.value < 0.4f)
				ans.level++;
			else
				break;
		}
		/** Random Stat Boosts */
		for (int i = 0; i >= 1; i ++) {
			if (Random.value < 0.5f)
				ans.AddBoost(Random.Range(0, StatData.NUM_STATS), 1);
			else
				break;
		}
		/** Random Tier? */
		return ans;
	}
}
