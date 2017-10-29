using UnityEngine;
using System.Collections;

public class PartyViewMenu : MonoBehaviour
{
	public PawnSelectionView pawnSelectionView;

	void Awake()
	{
		pawnSelectionView.Init();
	}
}
