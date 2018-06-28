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
	private Animator anim;			// Controls animating menu components

	private GameObject selectedStageIcon;

	[Header("Prefabs")]
	public GameObject stageSeriesIconPrefab;
	public GameObject stageIconPrefab;

	[Header("Stage Series Select View")]
	public TMP_Text stageSeriesNameText;
	public Transform stageSeriesIconFolder;

	[Header("Stage Select View")]
	public GameObject stageSelectionView;
	public Transform stageIconFolder;
	public GameObject highlightMenu;

	public delegate void OnStageIconSelected();
	public event OnStageIconSelected StageIconSelected;

	void Awake() {
		anim = GetComponent<Animator>();
	}

	void OnEnable()
	{
		DeselectStageIcon();
		stageSelectionView.SetActive(false);
	}

	public void InitStageSeriesSelectionView()
	{
		gm = GameManager.instance;
		StageCollectionData data = gm.regularStages;

		foreach (GameObject o in stageSeriesIcons)
			o.SetActive(false);
		int iconIndex = 0;			// Used to track the number of icons in the scene, and add more if needed 
		for (int i = 0; i <= gm.save.LatestSeriesIndex; i ++)
		{
			GameObject o;
			if (iconIndex >= stageSeriesIcons.Count)
			{
				o = Instantiate(stageSeriesIconPrefab);
				o.GetComponent<StageSeriesIcon>().onClicked += InitStageSelectionView;
				print ("Hello");
				stageSeriesIcons.Add(o);
			}
			else
			{
				o = stageSeriesIcons[iconIndex];
			}
			o.transform.SetParent(stageSeriesIconFolder, false);
			o.GetComponent<StageSeriesIcon>().Init(gm.regularStages.series[i]);
			o.SetActive(true);
			iconIndex++;
		}
	}

	public void InitStageSelectionView(StageSeriesIcon selectedIcon)
	{
		selectedIcon.clickable.interactable = false;
		StageSeriesData stageSeriesData = selectedIcon.GetData();
		foreach (GameObject seriesIcon in stageSeriesIcons) {
			if (seriesIcon.GetComponent<StageSeriesIcon>() != selectedIcon)
				seriesIcon.SetActive(false);
		}
		anim.SetFloat("Direction", 1.0f);
		anim.Play("ShowStageSelect", -1, 0);
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

	public void ResetStageSeriesMenu() {
		foreach (GameObject icon in stageSeriesIcons) {
			icon.SetActive(true);
			icon.GetComponent<StageSeriesIcon>().clickable.interactable = true;
		}
		anim.SetFloat("Direction", -1.0f);
		anim.Play("ShowStageSelect", -1, 1);
	}

	public void SelectStageIcon(GameObject stageIconObj)
	{    
		DeselectStageIcon();	// If there is already a selected stage icon, deselect it first
		selectedStageIcon = stageIconObj;	// store a reference to this object so that the Deselect method has a reference to it later
		StageIcon stageIcon = stageIconObj.GetComponent<StageIcon>();
		// Do the actual selection in the GameManager
		GameManager gm = GameManager.instance;
		gm.selectedSeriesIndex = stageIcon.seriesIndex;
		gm.selectedStageIndex = stageIcon.stageIndex;
		// UI Stuff
		highlightMenu.SetActive(true);
		selectedStageIcon.SetActive(false);
		highlightMenu.transform.SetParent(stageIconFolder, false);
		stageIcon.highlightMenu = highlightMenu;
		stageIcon.ExpandHighlightMenu();
		int siblingIndex = stageIcon.stageIndex;
		highlightMenu.transform.SetSiblingIndex(siblingIndex);

		if (StageIconSelected != null)
			StageIconSelected();
	}

	public void DeselectStageIcon()
	{
		// if there is nothing to deselect, return
		if (selectedStageIcon == null)
			return;

		selectedStageIcon.SetActive(true);
		StageIcon stageIcon = selectedStageIcon.GetComponent<StageIcon>();
		stageIcon.CollapseHighlightMenu();
		selectedStageIcon.transform.SetSiblingIndex(stageIcon.stageIndex);
		selectedStageIcon = null;
	}
}
