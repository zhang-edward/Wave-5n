//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class AcquirePawnsView : MonoBehaviour
//{
//	private List<Pawn> acquiredPawns = new List<Pawn>();
//	private Animator anim;
//	public HeroesRescuedMenu heroesRescuedMenu;
//	public UnityEngine.EventSystems.EventTrigger eventTrigger;

//	private bool proceed;

//	void Awake()
//	{
//		anim = GetComponent<Animator>();
//	}

//	void OnEnable()
//	{
//		heroesRescuedMenu.Reset();
//		acquiredPawns.Clear();
//	}

//	public void Init(params Pawn[] pawns)
//	{
//		gameObject.SetActive(true);
//		foreach (Pawn pawn in pawns)
//		{
//			this.acquiredPawns.Add(pawn);
//		}

//		if (pawns.Length > 1)
//			anim.CrossFade("Appear_Multiple", 0f);
//		else
//			anim.CrossFade("Appear", 0f);
//		StartCoroutine(AcquirePawnRoutine());
//	}

//	public void RevealPawnIcons()
//	{
//		heroesRescuedMenu.Init(acquiredPawns);
//	}

//	private IEnumerator AcquirePawnRoutine()
//	{
//		yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
//		yield return new WaitForEndOfFrame();
//		print("User pressed screen-animating");
//		anim.SetTrigger("Animate");
//		yield return new WaitForSeconds(2.5f);
//		yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
//		yield return new WaitForEndOfFrame();
//		print("User pressed screen-animating out");
//		anim.SetTrigger("AnimateOut");
//		yield return new WaitForSeconds(0.1f);      // wait for the animation state to update before continuing
//		gameObject.SetActive(false);
//	}
//}
