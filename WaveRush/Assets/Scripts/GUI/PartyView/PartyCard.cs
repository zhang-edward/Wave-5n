using UnityEngine;
using UnityEngine.UI;

public class PartyCard : MonoBehaviour {

	public Button button;
	public Image healthFill;
	public Image specialIcon;
	public Image specialIconFill;
	public PawnIconStandard icon;
	[HideInInspector] public int index;

	private PartyView partyView;

	public void Init(PartyView partyView, Pawn pawnData, float health, float specialCharge, int index) {
		this.partyView = partyView;
		icon.Init(pawnData);
		specialIcon.sprite = DataManager.GetHeroData(pawnData.type).specialAbilityIcon;	// Special ability icon index
		this.index = index;
		UpdateCard(health, specialCharge);

		button.onClick.AddListener(() => {
			partyView.SetHero(index);
		});
	}

	public void UpdateCard(float health, float specialCharge) {
		healthFill.fillAmount = health;
		specialIconFill.fillAmount = 1 - specialCharge;
	}
}