using UnityEngine;
using UnityEditor;

public class AcquireThingView : MonoBehaviour
{
	private Pawn pawn;
	public PawnIconReveal pawnIcon;

	public void Init(Pawn pawn)
	{
		this.pawn = pawn;
		pawnIcon.Init(pawn);
	}

	void Start()
	{
		Init(new Pawn(HeroType.Mage));
	}

	public void RevealPawnIcon()
	{
		pawnIcon.Reveal();
	}

}
