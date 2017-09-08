using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PawnInfoPanel : MonoBehaviour
{
	public PawnIconStandard pawnIcon;
	public TMP_Text damageText, healthText, livesText;
	public ScrollingText infoText;

	public Transform midPanel;
	public GameObject heroPowerUpInfoPrefab;
	public ToggleGroup infoIconsToggleGroup;
	private GameObject[] heroPowerUpInfoIcons;

	void Awake()
	{
		heroPowerUpInfoIcons = new GameObject[HeroPowerUpListData.powerUpUnlockLevels.Length];
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++)
		{
			GameObject o = Instantiate(heroPowerUpInfoPrefab);
			o.transform.SetParent(midPanel, false);
			o.GetComponent<ScrollingTextOption>().scrollingText = infoText;
			o.GetComponent<Toggle>().group = infoIconsToggleGroup;
			heroPowerUpInfoIcons[i] = o;
		}
	}

	void OnEnable()
	{
		infoText.SetToDefaultText();
		foreach(GameObject o in heroPowerUpInfoIcons)
		{
			o.GetComponent<Toggle>().isOn = false;
		}
	}

	public void Init(Pawn pawn)
	{
		pawnIcon.Init(pawn);
		HeroPowerUpListData powerUpListData = DataManager.GetPowerUpListData(pawn.type);
		int numPowerUpsUnlocked = HeroPowerUpListData.GetNumPowerUpsUnlocked(pawn.level);
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++)
		{
			bool locked = i >= numPowerUpsUnlocked;
			int unlockedLevel = HeroPowerUpListData.powerUpUnlockLevels[i];
			HeroPowerUpData data = powerUpListData.GetPowerUpFromIndex(i).data;
			heroPowerUpInfoIcons[i].GetComponent<HeroPowerUpInfoIcon>().Init(data, locked, unlockedLevel);
			heroPowerUpInfoIcons[i].GetComponent<ScrollingTextOption>().scrollingText = infoText;
		}
	}
}
