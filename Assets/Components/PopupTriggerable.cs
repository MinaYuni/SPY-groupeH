using UnityEngine;
using System.Collections.Generic;

public class PopupTriggerable : MonoBehaviour {
	
	public enum PopupType { Printer, LockedDoor, Note }; 
	public enum TriggerLocation { InFront, OnTop };

    public PopupType popupType;
    public TriggerLocation triggerLocation;
}