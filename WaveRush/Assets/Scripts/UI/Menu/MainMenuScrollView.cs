﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScrollView : ScrollViewSnap
{
	public CanvasScaler canvasScaler;
	public MusicTracksFader musicTracksFader;

	void Awake()
	{
		StartCoroutine(InitAfter1Frame());  // Due to some buggy shit with anchoredPositions in layouts at the start
	}

	void Start()
	{
		SoundManager.instance.PauseMusic();
	}

	void OnEnable()
	{
		OnSelectedContentChanged += StartFadeMusic;
	}

	private void StartFadeMusic()
	{
		musicTracksFader.StartFadeMusic(selectedContentIndex);
	}

	protected override void InitContent()
	{
		base.InitContent();
		contentDistance = Mathf.Abs(
			content[1].GetComponent<RectTransform>().anchoredPosition.x -
			content[0].GetComponent<RectTransform>().anchoredPosition.x);
		SetSelectedContentIndex(1);
		ForcePosition();
	}

	IEnumerator InitAfter1Frame()
	{
		yield return new WaitForEndOfFrame();
		Init();
	}
}
