using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GachaMenu : MonoBehaviour
{
	private GameManager gm;
	private const int SOULS_GACHA_COST = 5;

	public AcquirePawnsView acquirePawnsView;

	void Start()
	{
		gm = GameManager.instance;
	}

	public void TrySoulsGacha()
	{
		print("Trying souls gacha");
		if (gm.saveGame.pawnWallet.HasExtraPawns())
		{
			gm.DisplayAlert("You have too many heroes! Try fusing or retiring them.");
		}
		else
		{
			if (gm.wallet.TrySpendSouls(SOULS_GACHA_COST))
				SoulsGacha();
			else
				gm.DisplayAlert("Not Enough Souls!");
		}
	}

	public void SoulsGacha()
	{
		List<Pawn> acquiredPawns = new List<Pawn>();
		int level = 5;				// The overall level of the gacha (determines the levels of the heroes dropped)
		int numPawnsToGenerate = 1;	// Guaranteed 1 pawn to drop, maximum of 5 pawns to drop
		print("Got " + numPawnsToGenerate + " new pawns");
		level -= (int)Mathf.Sqrt(numPawnsToGenerate);	// scale the level by the number of pawns dropped
		for (int i = 0; i < numPawnsToGenerate; i ++)
		{
			acquiredPawns.Add(PawnGenerator.GenerateCrystalDrop(level));
		}
		foreach(Pawn pawn in acquiredPawns)
		{
			gm.saveGame.pawnWallet.AddPawn(pawn);
		}
		acquirePawnsView.Init(acquiredPawns.ToArray());
	}
}