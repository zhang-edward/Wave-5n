﻿﻿﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StageSelectMenu : MonoBehaviour
{
	private GameManager gm;         // store a reference to the GameManager
	private List<GameObject> stageSeriesIcons = new List<GameObject>();
	private List<GameObject> stageIcons = new List<GameObject>();

	private GameObject selectedStageIcon;

	[Header("Prefabs")]
	public GameObject stageSeriesIconPrefab;
	public GameObject stageIconPrefab;

	[Header("Stage Series Select View")]
	public TMP_Text stageSeriesNameText;
	public Transform stageSeriesIconFolder;
	public ScrollViewSnap stageSeriesScrollView;
	public GameObject leftButton, rightButton;

	[Header("Stage Select View")]
	public GameObject stageSelectionView;
	public Transform stageIconFolder;
	public Transform stageIconSelectedFolder;
	public GameObject placeholder;
	public GameObject highlightMenu;

	void OnEnable()
	{
		DeselectStageIcon();
		stageSelectionView.SetActive(false);
		StartCoroutine(UpdateScrollButtonsVisibility());
		stageSeriesScrollView.OnSelectedContentChanged += () => {
			stageSeriesNameText.text = stageSeriesScrollView.SelectedContent.GetComponent<StageSeriesIcon>().GetData().seriesName;
		};
	}

	void Awake()
	{
		InitStageSeriesSelectionView();
	}

	public void InitStageSeriesSelectionView()
	{
		gm = GameManager.instance;
		StageCollectionData data = gm.regularStages;

		foreach (GameObject o in stageSeriesIcons)
			o.SetActive(false);
		int iconIndex = 0;			// Used to track the number of icons in the scene, and add more if needed 
		for (int i = 0; i <= gm.saveGame.latestUnlockedSeriesIndex; i ++)
		{
			GameObject o;
			if (iconIndex >= stageSeriesIcons.Count)
			{
				o = Instantiate(stageSeriesIconPrefab);
				o.GetComponent<StageSeriesIcon>().onClicked += InitStageSelectionView;
				stageSeriesIcons.Add(o);
			}
			else
			{
				o = stageSeriesIcons[iconIndex];
			}
			stageSeriesScrollView.content.Add(o);
			o.transform.SetParent(stageSeriesIconFolder, false);
			o.GetComponent<StageSeriesIcon>().Init(gm.regularStages.series[i]);
			o.SetActive(true);
			iconIndex++;
		}
		stageSeriesNameText.text = stageSeriesScrollView.SelectedContent.GetComponent<StageSeriesIcon>().GetData().seriesName;
		StartCoroutine(InitScrollViewAfter1Frame());
	}

	private IEnumerator InitScrollViewAfter1Frame()
	{
		yield return new WaitForEndOfFrame();
		stageSeriesScrollView.Init();
	}

	private IEnumerator UpdateScrollButtonsVisibility()
	{
		for (;;)
		{
			leftButton.SetActive(stageSeriesScrollView.selectedContentIndex > 0);
			rightButton.SetActive(stageSeriesScrollView.selectedContentIndex < stageSeriesScrollView.content.Count - 1);
			yield return null;
		}
	}

	public void InitStageSelectionView(StageSeriesData stageSeriesData)
	{
		stageSelectionView.gameObject.SetActive(true);
		foreach (GameObject o in stageIcons)
		{
			o.SetActive(false);
		}
		UnityEngine.Assertions.Assert.IsTrue(gm.IsSeriesUnlocked(stageSeriesData.seriesName));
		int numStagesUnlocked = gm.NumStagesUnlocked(stageSeriesData.seriesName);
		int iconIndex = 0;								// Used to track the number of icons in the scene, and add more if needed 
		for (int i = 0; i < numStagesUnlocked; i ++)
		{
			GameObject o;
			if (iconIndex >= stageIcons.Count)
			{
				// Instantiate and initialize a new stage icon prefab
				o = Instantiate(stageIconPrefab);
				stageIcons.Add(o);
				StageIcon icon = o.GetComponent<StageIcon>();
				//icon.highlightMenu.GetComponent<Button>().onClick.AddListener(() => DeselectStageIcon());
			}
			else
			{
				o = stageIcons[iconIndex];
			}
			o.transform.SetParent(stageIconFolder, false);
			o.SetActive(true);

			StageData stageData = stageSeriesData.stages[i];
			StageIcon stageIcon = o.GetComponent<StageIcon>();
			stageIcon.seriesIndex = stageSeriesData.index;
			stageIcon.stageIndex = i;
			stageIcon.onClicked = SelectStageIcon;
			stageIcon.Init(stageData);
			iconIndex++;
		}
	}

	public void SelectStageIcon(GameObject stageIconObj)
	{
		selectedStageIcon = stageIconObj;	// store a reference to this object so that the Deselect method has a reference to it later
		StageIcon stageIcon = stageIconObj.GetComponent<StageIcon>();
		// Do the actual selection in the GameManager
		GameManager gm = GameManager.instance;
		gm.selectedSeriesIndex = stageIcon.seriesIndex;
		gm.selectedStageIndex = stageIcon.stageIndex;
		// UI Stuff
		highlightMenu.transform.SetParent(stageIcon.transform, false);
		stageIcon.highlightMenu = highlightMenu;
		stageIcon.ExpandHighlightMenu();
		stageIconSelectedFolder.gameObject.SetActive(true);
		placeholder.SetActive(true);
		int siblingIndex = stageIcon.stageIndex;
		placeholder.transform.SetSiblingIndex(siblingIndex);
		stageIconObj.transform.SetParent(stageIconSelectedFolder, false);
	}

	public void DeselectStageIcon()
	{
		// if there is nothing to deselect, return
		if (selectedStageIcon == null)
			return;
		
		StageIcon stageIcon = selectedStageIcon.GetComponent<StageIcon>();
		stageIcon.CollapseHighlightMenu();
		stageIconSelectedFolder.gameObject.SetActive(false);
		placeholder.SetActive(false);
		selectedStageIcon.transform.SetParent(stageIconFolder, false);
		selectedStageIcon.transform.SetSiblingIndex(stageIcon.stageIndex);
		selectedStageIcon = null;
	}
}
