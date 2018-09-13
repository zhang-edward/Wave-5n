using UnityEngine;

public class PartyView : MonoBehaviour {

	public GameObject partyCardPrefab;
	public GameObject placeholderCardPrefab;

	private PartyCard[] partyCards;
	private GameObject[] placeholderCards;
	public Transform contentFolder;
	public Transform placeholdersFolder;
	
	public delegate void OnSwitchHero(int index);
	public event OnSwitchHero SwitchHero;

	public void Init(Pawn[] party) {
		partyCards = new PartyCard[party.Length];
		placeholderCards = new GameObject[party.Length];
		for (int i = 0; i < party.Length; i ++) {
			//CreatePlaceholderCard(i);
			CreatePartyCard(i, party[i]);
		}
	}

	private void CreatePartyCard(int index, Pawn pawn) {
		GameObject o = Instantiate(partyCardPrefab);
		o.transform.SetParent(contentFolder, false);
		PartyCard card = o.GetComponent<PartyCard>();
		partyCards[index] = card;
		card.Init(this, pawn, 1.0f, 0.0f, index);
		// First hero is the one on screen, so disable its card
		if (index == 0)
			DeactivateCard(index);
	}

	private void CreatePlaceholderCard(int index) {
		GameObject o = Instantiate(placeholderCardPrefab);
		o.transform.SetParent(placeholdersFolder, false);
		placeholderCards[index] = o;
		o.gameObject.SetActive(false);
	}

	public void SetHero(int index) {
		if (SwitchHero != null)
			SwitchHero(index);
	}

	public void UpdatePartyCard(int index, float health, float specialCharge) {
		partyCards[index].UpdateCard(health, specialCharge);
	}

	public void DeactivateCard(int index) {
		partyCards[index].button.interactable = false;
		// GameObject placeholder = placeholderCards[index];
		// placeholder.gameObject.SetActive(true);
		// placeholder.transform.SetParent(contentFolder);
		// placeholder.transform.SetSiblingIndex(index);
	}

	public void ActivateCard(int index) {
		partyCards[index].button.interactable = true;
		// GameObject placeholder = placeholderCards[index];
		// placeholder.transform.SetParent(placeholdersFolder);
		// placeholder.SetActive(false);
	}
}