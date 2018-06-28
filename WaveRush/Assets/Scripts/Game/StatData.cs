using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Player/StatData", order = 0)]
public class StatData : ScriptableObject {
	
	public const int STR  = 0;
	public const int VIT  = 1;
	public const int CHG  = 2;
	public const int DEX  = 3;
	public const int CRIT = 4;
	public const int LUCK = 5;
	public const int NUM_STATS = 6;

	[Header("Descriptions")]
	[TextArea]
	public string[] statDescriptions;
}
