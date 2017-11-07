using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PawnIcon : MonoBehaviour
{
	public Pawn pawnData; //{ get; private set; }

	public virtual void Init(Pawn pawnData)
	{
		this.pawnData = pawnData;
	}
}
