using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GachaMenu : MonoBehaviour
{
	private GameManager gm;

	public AcquirePawnsView acquirePawnsView;

	void Start()
	{
		gm = GameManager.instance;
	}

	public void TrySoulsGacha()
	{
		if (gm.saveGame.HasExtraPawns())
		{
			gm.DisplayAlert("You have too many heroes! Try fusing or retiring them.");
		}
		else
		{
			SoulsGacha();
		}
	}

	public void SoulsGacha()
	{
		List<Pawn> acquiredPawns = new List<Pawn>();
		int level = 1;				// The overall level of the gacha (determines the levels of the heroes dropped)
		float spawnChance = 0.5f;	// Initial spawn chance for an additional pawn
		int numPawnsToGenerate = 1;	// Guaranteed 1 pawn to drop, maximum of 5 pawns to drop
		for (int i = 0; i < 4; i++)
		{
			if (Random.value < spawnChance)
			{
				numPawnsToGenerate++;
				spawnChance *= 0.9f;		// Spawn chance multiplier
			}
			else
			{
				break;
			}
		}
		print("Got " + numPawnsToGenerate + " new pawns");
		level -= (int)Mathf.Sqrt(numPawnsToGenerate);	// scale the level by the number of pawns dropped
		for (int i = 0; i < numPawnsToGenerate; i ++)
		{
			acquiredPawns.Add(PawnGenerator.GenerateCrystalDrop(level));
		}
		foreach(Pawn pawn in acquiredPawns)
		{
			gm.saveGame.AddPawn(pawn);
		}
		acquirePawnsView.Init(acquiredPawns.ToArray());
	}
}
