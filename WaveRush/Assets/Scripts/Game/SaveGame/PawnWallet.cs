using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PawnWallet
{
	public const int INITIAL_PAWN_CAPACITY = 10;

	public int numPawns;							// how many pawns the player has
	public int pawnCapacity { get; private set; }	// the total amount of pawns that the player can possess at one time
	public Pawn[] pawns { get; private set; }	// the master list of the different pawns the player possesses

	public PawnWallet()
	{
		pawnCapacity = INITIAL_PAWN_CAPACITY;
		pawns = new Pawn[pawnCapacity];
	}

	public bool AddPawn(Pawn pawn, out int id)
	{
		for (int i = 0; i < pawns.Length; i++) {
			if (pawns[i] == null) {
				id = i;
				pawn.SetID(id);
				pawns[id] = pawn;
				Debug.Log("New Pawn:" + pawn);
				return true;
			}
		}
		Debug.LogError("Add Pawn Failed");
		id = -1;
		return false;
	}

	public Pawn GetPawn(int id)
	{
		if (id < pawnCapacity)
			return pawns[id];
		return null;
	}

	public bool RemovePawn(int id)
	{
		if (id < pawnCapacity)
		{
			if (pawns[id] != null)
			{
				Debug.Log("Removed Pawn:" + pawns[id]);
				pawns[id] = null;
				return true;
			}
		}
		return false;
	}

	public int AddExperience(int id, int amt) {
		if (id < pawnCapacity) {
			if (pawns[id] != null) {
				Debug.Log("Added " + amt + " experience to pawn " + pawns[id]);
				return pawns[id].AddExperience(amt);
			}
		}
		return -1;
	}

	public void LoseExperience(int id, int amt) {
		if (id < pawnCapacity) {
			if (pawns[id] != null) {
				Debug.Log("Lost " + amt + " experience for pawn " + pawns[id]);
				pawns[id].LoseExperience(amt);
			}
		}
	}

	public bool HasPawns() {
		for (int i = 0; i < pawns.Length; i ++)
		{
			if (pawns[i] != null)
				return true;
		}
		return false;
	}

	public void ChangePawnCapacity(int newCapacity) {
		UnityEngine.Assertions.Assert.IsTrue(newCapacity > pawnCapacity);
		pawnCapacity = newCapacity;
		Pawn[] newPawns = new Pawn[newCapacity];
		pawns.CopyTo(newPawns, 0);
		pawns = newPawns;
	}
}
