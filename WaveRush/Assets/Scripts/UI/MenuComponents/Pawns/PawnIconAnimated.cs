using UnityEngine;
using System.Collections;

public class PawnIconAnimated : PawnIconStandard
{
	private const string STATE_LEVELUP = "LevelUp";

	[Header("Audio")]
	public AudioClip gainExperienceBlip;
	public AudioClip levelUpSound;

	private Animator anim;
	private Pawn endState;

	private Coroutine gainExperienceBlipRoutine;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	public void Init(Pawn startState, Pawn endState)
	{
		base.Init(startState);
		this.endState = endState;
	}

	public void PlayAnim(string state)
	{
		anim.Play(state, 0, 0);
	}

	public void AnimateGetExperience()
	{
		StartCoroutine(AnimateGetExperienceRoutine());
	}


	private IEnumerator AnimateGetExperienceRoutine()
	{
		int levelCounter = pawnData.level;
		int numLevelUps = endState.level - pawnData.level;
		float endingExp = endState.Experience;
		// gainExperienceBlipRoutine = StartCoroutine(GainExperienceBlipRoutine());
		while (numLevelUps > 0)
		{
			while (Mathf.Abs(experienceSlider.value - experienceSlider.maxValue) > 0.2f)
			{
				experienceSlider.value += experienceSlider.maxValue / 64;
				yield return null;
			}
			PlayAnim(STATE_LEVELUP);
			SoundManager.instance.PlaySingle(levelUpSound);
			SetLevel(++levelCounter);
			experienceSlider.value = 0;
			numLevelUps--;
		}
		// StopCoroutine(gainExperienceBlipRoutine);
		experienceSlider.maxValue = endState.MaxExperience;
		while (Mathf.Abs(experienceSlider.value - endingExp) > 0.2f)
		{
			//experienceSlider.value += experienceSlider.maxValue / 64;
			experienceSlider.value = Mathf.Lerp(experienceSlider.value, endingExp, 0.2f);
			yield return null;
		}

		Init(endState);
	}

	private IEnumerator GainExperienceBlipRoutine()
	{
		for(;;)
		{
			SoundManager.instance.RandomizeSFX(gainExperienceBlip);
			yield return new WaitForSeconds(0.2f);
		}
	}
}
