using UnityEngine;
[System.Serializable]
public class PawnWallet
{
	public const int INITIAL_PAWN_CAPACITY = 20;

	public int pawnCapacity { private get; set; }	// the total amount of pawns that the player can possess at one time
	public int numPawns;							// how many pawns the player has
	public Pawn[] pawns { get; private set; }		// the master list of the different pawns the player possesses
	public Pawn[] extraPawns { get; private set; }	// a list of extra pawns acquired - must be empty to enter battle

	public PawnWallet()
	{
		pawnCapacity = INITIAL_PAWN_CAPACITY;
		pawns = new Pawn[pawnCapacity];
		extraPawns = new Pawn[10];
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
//				Debug.Log("New Pawn:" + pawn + " with unlock time:" + unlockTime);
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
		while (debugCounter < 10)       // if we are trying to add 1024+ extra pawns (10 resizings = 2^10 = 1024)
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
			for (int i = 0; i < extraPawns.Length; i++)
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
					Debug.Log("Extra pawn filled slot " + id + ":" + extraPawns[i]);
					AddPawn(extraPawns[i], false, extraPawns[i].unlockTime);
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

	public bool HasPawns()
	{
		for (int i = 0; i < pawns.Length; i ++)
		{
			if (pawns[i] != null)
				return true;
		}
		return false;
	}
}
