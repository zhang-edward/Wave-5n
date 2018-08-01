using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// Modifies the save file and broadcasts events. Used so no events are stored (and serialized) 
/// in the SaveGame class.
/// </summary>
public class SaveModifier {

	public int money { get { return wallet.money; } }
	public int souls { get { return wallet.souls; } }
	public Pawn[] pawns { get { return pawnWallet.pawns; } }

	public int LatestSeriesIndex { get { return sg.saveDict[SaveGame.LATEST_UNLOCKED_SERIES_INDEX_KEY]; } }
	public int LatestStageIndex  { get { return sg.saveDict[SaveGame.LATEST_UNLOCKED_STAGE_INDEX_KEY] ; } }
	public bool[] UnlockedHeroes { get { return sg.unlockedHeroes; } }
	public List<Pawn> AvailableHeroes { get { return sg.availableHeroes; } set { sg.availableHeroes = value; }  }

	private SaveGame sg;
	private Wallet wallet;
	private PawnWallet pawnWallet;

	public delegate void SaveStateUpdate();
	public SaveStateUpdate OnHasViewedDictionaryUpdated;
	public delegate void PawnStateUpdate(int id);
	public PawnStateUpdate OnPawnListUpdated;


	public SaveModifier(SaveGame saveGame) {
		sg = saveGame;
		wallet = sg.wallet;
		pawnWallet = sg.pawnWallet;
	}

	//public int GetValue(string key) {
	//	if (sg.saveDict.ContainsKey(key))
	//		return sg.saveDict[key];
	//	else {
	//		Debug.LogWarning("Couldn't find key \"" + key + "\"");
	//		return int.MaxValue;
	//	}
	//}

	public void SetValue(string key, int value) {
		sg.saveDict[key] = value;
	}

	/// <summary>
	/// Sets the key in the viewed dictionary to the given value
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="val">value to set</param>
	// public void SetHasPlayerViewedKey(string key, bool val) {
	// 	//		print("Key: " + key);
	// 	if (!sg.hasPlayerViewedDict.ContainsKey(key))
	// 		sg.hasPlayerViewedDict.Add(key, val);
	// 	else
	// 		sg.hasPlayerViewedDict[key] = val;
	// 	// Event for NewFeatureIndicators to refresh their 
	// 	if (OnHasViewedDictionaryUpdated != null)
	// 		OnHasViewedDictionaryUpdated();
	// }

	// public void InitHasPlayerViewedKey(string key, bool val) {
	// 	Assert.IsTrue(!sg.hasPlayerViewedDict.ContainsKey(key));
	// 	sg.hasPlayerViewedDict.Add(key, val);
	// }

	/** PawnWallet Modifiers */
	public bool AddPawn(Pawn pawn) {
		int id;
		if (pawnWallet.AddPawn(pawn, out id)) {
			if (OnPawnListUpdated != null)
				OnPawnListUpdated(id);
			return true;
		}
		return false;
	}

	public bool RemovePawn(int id) {
		if (pawnWallet.RemovePawn(id)) {
			if (OnPawnListUpdated != null)
				OnPawnListUpdated(id);
			return true;
		}
		return false;
	}

	public int AddExperience(int id, int amt) {
		return pawnWallet.AddExperience(id, amt);
	}

	public void LoseExperience(int id, int amt) {
		pawnWallet.LoseExperience(id, amt);
	}

	/** PawnWallet Getters */
	public int NumPawns() {
		return pawnWallet.NumPawns();
	}

	public Pawn GetPawn(int id) {
		return pawnWallet.GetPawn(id);
	}

	/** Wallet Modifiers */
	public bool TrySpendMoney(int amt) {
		return wallet.TrySpendMoney(amt);
	}

	public bool TrySpendSouls(int amt) {
		return wallet.TrySpendSouls(amt);
	}

	public void AddMoney(int amt) {
		wallet.AddMoney(amt);
	}

	public void AddSouls(int amt) {
		wallet.AddSouls(amt);
	}

	public void SetMoneyDebug(int amt) {
		wallet.SetMoney(amt);
	}

	public void SetSoulsDebug(int amt) {
		wallet.SetSouls(amt);
	}

	public void UnlockHero(int index) {
		sg.unlockedHeroes[index] = true;
	}
}
