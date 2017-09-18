using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AcquirePawnsView : MonoBehaviour
{
	private List<Pawn> acquiredPawns = new List<Pawn>();
	private Animator anim;
	public HeroesRescuedMenu heroesRescuedMenu;
	public UnityEngine.EventSystems.EventTrigger eventTrigger;

	private bool proceed;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	void OnEnable()
	{
		heroesRescuedMenu.Reset();
		acquiredPawns.Clear();
	}

	public void Init(params Pawn[] pawns)
	{
		gameObject.SetActive(true);
		foreach (Pawn pawn in pawns)
		{
			this.acquiredPawns.Add(pawn);
		}

		if (pawns.Length > 1)
			anim.CrossFade("Appear_Multiple", 0f);
		else
			anim.CrossFade("Appear", 0f);
		StartCoroutine(AcquirePawnRoutine());
	}

	public void RevealPawnIcons()
	{
		heroesRescuedMenu.Init(acquiredPawns);
	}

	private IEnumerator AcquirePawnRoutine()
	{
		yield return new WaitUntil(() => proceed);
		proceed = false;
		anim.SetTrigger("Animate");
		yield return new WaitUntil(() => heroesRescuedMenu.AllIconsRevealed() && proceed);
		proceed = false;
		anim.SetTrigger("AnimateOut");
		yield return new WaitForSeconds(0.1f);      // wait for the animation state to update before continuing
		gameObject.SetActive(false);
	}

	public void Proceed()
	{
		StartCoroutine(ProceedRoutine());
	}

	private IEnumerator ProceedRoutine()
	{
		proceed = true;
		yield return null;
		proceed = false;
	}
}
