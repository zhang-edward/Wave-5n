using UnityEngine;
using UnityEngine.UI;

public class CycledButton : MonoBehaviour {

	public Button button;
	public int maxCycleIndex = 2;
	public int cycleIndex { get; set; }
	
	public delegate void ButtonPressed(int cycleIndex);
	public event ButtonPressed OnButtonPressed;

	void Awake() {
		button = GetComponent<Button>();
		button.onClick.AddListener(OnButtonClicked);
	} 

	private void OnButtonClicked() {
		cycleIndex = (cycleIndex + 1) % maxCycleIndex;
		if (OnButtonPressed != null)
			OnButtonPressed(cycleIndex);
	}

}