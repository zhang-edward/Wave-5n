using UnityEngine;
using System.Collections.Generic;

public class PawnShop {

	private List<Pawn> pawnPool = new List<Pawn>();		// The list of pawns which AvailablePawns selects from
	public List<Pawn> AvailablePawns { get; private set; }	// The available pawns in the pawn shop

	public SaveModifier.PawnStateUpdate OnPawnListUpdated;

	/// <summary>
	/// Initializes a new instance of the <see cref="PawnShop"/> class.
	/// </summary>
	public PawnShop() {
		AvailablePawns = new List<Pawn>();
	}

	/// <summary>
	/// Called when the save game is loaded. Initializes <see cref="Unlocked"/> array
	/// </summary>
	/// <param name="save">Save.</param>
	public void OnSaveGameLoaded(SaveModifier save) {
		
		bool[] unlockedHeroes = save.UnlockedHeroes;
		for (int i = 0; i < unlockedHeroes.Length; i++) {
			if (unlockedHeroes[i]) {
				HeroType type = (HeroType)(i / 3);
				HeroTier tier = (HeroTier)(i % 3);
				pawnPool.Add(new Pawn(type, tier));
			}
		}
		for (int i = 0; i < 5; i ++) {
			// Randomly modify a pawn from the pawn pool, then add it to the available pawns
			AvailablePawns.Add(RandomlyModify(pawnPool[Random.Range(0, pawnPool.Count)]));
		}
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
		/** Random Tier? */
		return ans;
	}
}
