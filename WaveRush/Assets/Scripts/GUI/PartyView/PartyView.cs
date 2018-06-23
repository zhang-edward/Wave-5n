using UnityEngine;

public class PartyView : MonoBehaviour {

	public GameObject partyCardPrefab;

	private PartyCard[] partyCards;
	
	public delegate void OnSwitchHero(int index);
	public event OnSwitchHero SwitchHero;

	public void Init(Pawn[] party) {
		partyCards = new PartyCard[party.Length];
		for (int i = 0; i < party.Length; i ++) {
			GameObject o = Instantiate(partyCardPrefab);
			o.transform.SetParent(transform, false);
			PartyCard card = o.GetComponent<PartyCard>();
			partyCards[i] = card;
			card.Init(this, party[i], 1.0f, 0.0f, i);
			if (i == 0)
				o.SetActive(false);
		}
	}

	public void SetHero(int index) {
		if (SwitchHero != null)
			SwitchHero(index);
	}

	public void UpdatePartyCard(int index, float health, float specialCharge) {
		partyCards[index].UpdateCard(health, specialCharge);
	}

	public void DeactivateCard(int index) {
		partyCards[index].gameObject.SetActive(false);
	}

	public void ActivateCard(int index) {
		partyCards[index].gameObject.SetActive(true);
	}
}