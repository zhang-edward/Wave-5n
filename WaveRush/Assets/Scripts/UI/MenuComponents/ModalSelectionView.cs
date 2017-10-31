using UnityEngine;
using System.Collections;

public class ModalSelectionView : MonoBehaviour
{
	public delegate void OptionSelected(int selection);
	public event OptionSelected OnOptionSelected; 

	public void SelectOption(int selection) 
	{
		if (OnOptionSelected != null)
			OnOptionSelected(selection);
	}
}
