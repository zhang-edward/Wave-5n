﻿﻿﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageSelectView : MonoBehaviour
{
	private GameManager gm;         // store a reference to the GameManager
	private List<GameObject> stageSeriesIcons = new List<GameObject>();
	private List<GameObject> stageIcons = new List<GameObject>();

	private GameObject selectedStageIcon;

	[Header("Prefabs")]
	public GameObject stageSeriesIconPrefab;
	public GameObject stageIconPrefab;

	[Header("Stage Series Select View")]
	public Text stageSeriesNameText;
	public Transform stageSeriesIconFolder;
	public ScrollViewSnap stageSeriesScrollView;

	[Header("Stage Select View")]
	public GameObject stageSelectionView;
	public Transform stageIconFolder;
	public Transform stageIconSelectedFolder;
	public GameObject placeholder;

	void Start()
	{
		InitStageSeriesSelectionView();
	}

	public void InitStageSeriesSelectionView()
	{
		gm = GameManager.instance;
		StageCollectionData data = gm.regularStages;

		foreach (GameObject o in stageSeriesIcons)
			o.SetActive(false);
		int i = 0;
		foreach (StageSeriesData stageSeriesData in data.stages)
		{
			if (gm.saveGame.IsStageUnlocked(stageSeriesData.seriesName))
			{
				GameObject o;
				if (i >= stageSeriesIcons.Count)
				{
					o = Instantiate(stageSeriesIconPrefab);
					o.GetComponent<StageSeriesIcon>().onClicked += InitStageSelectionView;
					stageSeriesIcons.Add(o);
				}
				else
				{
					o = stageSeriesIcons[i];
				}
				stageSeriesScrollView.content.Add(o);
				o.transform.SetParent(stageSeriesIconFolder, false);
				o.GetComponent<StageSeriesIcon>().Init(stageSeriesData);
				o.SetActive(true);
				i++;
			}
		}
		stageSeriesScrollView.Init();
	}

	public void InitStageSelectionView(StageSeriesData stageSeriesData)
	{
		stageSelectionView.gameObject.SetActive(true);
		foreach (GameObject o in stageIcons)
		{
			o.SetActive(false);
		}
		UnityEngine.Assertions.Assert.IsTrue(gm.saveGame.IsStageUnlocked(stageSeriesData.seriesName));
		int numStagesUnlocked = gm.saveGame.unlockedStages[stageSeriesData.seriesName];
		int j = 0;
		for (int i = 0; i < numStagesUnlocked; i ++)
		{
			GameObject o;
			if (j >= stageIcons.Count)
			{
				o = Instantiate(stageIconPrefab);
				stageIcons.Add(o);
				StageIcon icon = o.GetComponent<StageIcon>();
				icon.highlightMenu.GetComponent<Button>().onClick.AddListener(() => DeselectStageIcon());
			}
			else
			{
				o = stageIcons[j];
			}
			o.transform.SetParent(stageIconFolder, false);
			o.SetActive(true);
			StageIcon stageIcon = o.GetComponent<StageIcon>();
			stageIcon.onClicked = SelectStageIcon;
			stageIcon.Init(stageSeriesData.stages[i], i);
			j++;
		}
	}

	public void SelectStageIcon(GameObject stageIconObj)
	{
		StageIcon stageIcon = stageIconObj.GetComponent<StageIcon>();
		stageIcon.ExpandHighlightMenu();
		selectedStageIcon = stageIconObj;
		stageIconSelectedFolder.gameObject.SetActive(true);
		placeholder.SetActive(true);
		int siblingIndex = stageIcon.index;
		placeholder.transform.SetSiblingIndex(siblingIndex);
		stageIconObj.transform.SetParent(stageIconSelectedFolder, false);
		print("Selected");
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
		selectedStageIcon.transform.SetSiblingIndex(stageIcon.index);
		selectedStageIcon = null;
		print("Deselected");
	}
}
