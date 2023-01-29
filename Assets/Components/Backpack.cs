using UnityEngine;
using System.Collections.Generic;

public class Backpack : MonoBehaviour {
	// Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
	public List<(string, int)> available_slots;

}