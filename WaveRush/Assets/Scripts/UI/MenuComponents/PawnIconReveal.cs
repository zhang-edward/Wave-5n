﻿using UnityEngine;
using System.Collections;

public class PawnIconReveal : PawnIconStandard
{
	[Header("Animation States")]
	public string revealSpecialState = "RevealSpecial";
	public string revealState = "Reveal";
	public bool revealed { get; private set; }

	private string reveal;
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	public override void Init(Pawn pawnData)
	{
		base.Init(pawnData);
		if (pawnData.tier == HeroTier.tier3)
		{
			reveal = revealSpecialState;
		}
		else
		{
			reveal = revealState;
		}
		revealed = false;
	}

	public void Reveal()
	{
		anim.CrossFade(reveal, 0);
		revealed = true;
	}
}
