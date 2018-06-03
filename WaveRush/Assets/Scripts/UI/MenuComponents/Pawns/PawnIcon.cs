using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PawnIcon : MonoBehaviour, System.IComparable<PawnIcon>
{
	public Pawn pawnData; //{ get; private set; }

	public virtual void Init(Pawn pawnData)
	{
		this.pawnData = pawnData;
	}

	public int CompareTo(PawnIcon other) {
		if (other == null)
			return 1;

		return (pawnData.CompareTo(other.pawnData));
	}
}
