using UnityEngine;
using System.Collections;

public class PawnShopMenu : MonoBehaviour
{
	GameManager gm;

	private PawnShop pawnShop;

	void Awake() {
		pawnShop = new PawnShop();
	}

	void Start() {
		gm = GameManager.instance;
		pawnShop.OnSaveGameLoaded(gm.save);
	}
}
