using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class ButtonBouncy : Button, IPointerDownHandler, IPointerUpHandler {

	private Animator anim;

	protected override void Awake() {
		base.Awake();
		anim = GetComponent<Animator>();
	}

	public override void OnPointerDown(PointerEventData eventData) {
		base.OnPointerDown(eventData);
		anim.SetTrigger("down");
	}

	public override void OnPointerUp(PointerEventData eventData) {
		base.OnPointerUp(eventData);
		anim.SetTrigger("up");
	}
}