using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AcquirePawnsView : MonoBehaviour
{
	private List<Pawn> acquiredPawns = new List<Pawn>();
	public HeroesRescuedMenu heroesRescuedMenu;

	public void Init(params Pawn[] pawns)
	{
		foreach(Pawn pawn in pawns)
		{
			this.acquiredPawns.Add(pawn);
		}

	}

	/*void Start()
	{
		Init(new Pawn(HeroType.Mage), new Pawn(HeroType.Mage));
	}*/

	public void RevealPawnIcons()
	{
		heroesRescuedMenu.Init(acquiredPawns);
	}
}
