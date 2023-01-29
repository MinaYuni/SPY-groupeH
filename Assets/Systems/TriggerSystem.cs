using UnityEngine;
using FYFY;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System;

public class TriggerSystem : FSystem {

	public static TriggerSystem instance;

	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));
	//private Family f_door = FamilyManager.getFamily(new AnyOfTags("LockedDoor"));
	private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));
	private Family f_triggerables = FamilyManager.getFamily(new AllOfComponents(typeof(PopupTriggerable)));

	public GameObject dialogPanel;
	public GameObject inputPanel;
	private bool popped;

	public TriggerSystem(){
		instance = this;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){
		GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, false);
		GameObjectManager.setGameObjectState(inputPanel.transform.parent.gameObject, false);
		popped = false;
	} 

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

		foreach (GameObject player in f_players){	
			
			Position robotPos = player.GetComponent<Position>();
			int robotDirection = (int) player.GetComponent<Direction>().direction;

			foreach (GameObject triggerable in f_triggerables){

				Position triggerablePos = triggerable.GetComponent<Position>();
				int triggerableDirection = (int) triggerable.GetComponent<Direction>().direction;
				bool triggered = false;

				switch (triggerable.GetComponent<PopupTriggerable>().triggerLocation){
					case PopupTriggerable.TriggerLocation.InFront:
						if (playerFacesTriggerable(robotDirection, robotPos.x, robotPos.y, triggerableDirection, triggerablePos.x, triggerablePos.y)){
							triggered = true;
						}
						break;
					case PopupTriggerable.TriggerLocation.OnTop:
						if (playerOnTriggerable(robotPos.x, robotPos.y, triggerablePos.x, triggerablePos.y)){
							triggered = true;
						}
						break;
				}

				if (triggered){
					switch (triggerable.GetComponent<PopupTriggerable>().popupType){
						case PopupTriggerable.PopupType.Printer:
							popped = trigger3DPrinter(triggerable);
							break;
						case PopupTriggerable.PopupType.LockedDoor:
							popped = triggerLockedDoor(triggerable);
							break;
						case PopupTriggerable.PopupType.Note:
							configureDialog("Il semblerait que quelqu'un a oublié ça ici...");
							popped = true;
							break;
					}
				}
			}
		}
	}

	private bool playerFacesTriggerable(int robot_ori, int robot_posX, int robot_posY, int triggerable_ori, int triggerable_posX, int triggerable_posY){
		
		if (triggerable_ori == 0 || triggerable_ori == 1){
			if (robot_ori == 2 || robot_ori == 3 || robot_posX != triggerable_posX){
				return false;
			} else if ((robot_posY == triggerable_posY - 1 && robot_ori == 1)||(robot_posY == triggerable_posY + 1 && robot_ori == 0)){
				return true;
			}

		} else {
			if (robot_ori == 0 || robot_ori == 1 || robot_posY != triggerable_posY){
				return false;
			} else if ((robot_posX == triggerable_posX - 1 && robot_ori == 2)||(robot_posX == triggerable_posX + 1 && robot_ori  == 3)){
				return true;
			}
		}

		return false;
	}

	private bool playerOnTriggerable(int robot_posX, int robot_posY, int triggerable_posX, int triggerable_posY){
		
		if (robot_posX == triggerable_posX && robot_posY == triggerable_posY){
			return true;
		} 

		return false;
	}

	private void configureDialog(string popup){
		// set text
		if (popped == false){
			GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, true);
			GameObject textGO = dialogPanel.transform.Find("Text").gameObject;
			// if (gameData.triggerMessage[nDialog].Item1 != null)
			// {
			GameObjectManager.setGameObjectState(textGO, true);
			textGO.GetComponent<TextMeshProUGUI>().text = popup;
			LayoutRebuilder.ForceRebuildLayoutImmediate(textGO.transform as RectTransform);
			// }
			// else
			// 	GameObjectManager.setGameObjectState(textGO, false);
			
			// set camera pos
			// if (gameData.dialogMessage[nDialog].Item4 != -1 && gameData.dialogMessage[nDialog].Item5 != -1)
			// {
			// 	GameObjectManager.addComponent<FocusCamOn>(MainLoop.instance.gameObject, new { camX = gameData.dialogMessage[nDialog].Item4, camY = gameData.dialogMessage[nDialog].Item5 });
			// }
			setActiveOKButton(true);
		}

	}

	public void setActiveOKButton(bool active){
		GameObjectManager.setGameObjectState(dialogPanel.transform.Find("Buttons").Find("OKButton").gameObject, active);
	}

	public void closeDialogPanel(){
		GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, false);
	}

	private bool trigger3DPrinter(GameObject printer){
		
		configureDialog("Je suis une imprimante 3D. Quelle clé veux-tu imprimer ?");
		
		GameObjectManager.setGameObjectState(inputPanel.transform.parent.gameObject, true);
		GameObject inputGO = inputPanel.transform.Find("TMP_InputField").gameObject;
		//Debug.Log("test: "+inputGO.ToString());
		GameObjectManager.setGameObjectState(inputGO, true);
		//inputGO.GetComponent<TMP_InputField>().text
		LayoutRebuilder.ForceRebuildLayoutImmediate(inputGO.transform as RectTransform);
		return true;
	}

	/* Triggers popup if the door is locked door and the robot doesn't have the key.  */
	private bool triggerLockedDoor(GameObject lockedDoor){

		// Nothing to do if door is open
		if (!lockedDoor.activeSelf) {
			return false;
		}
		
		// If door is locked, check backpack for matching key
		var backp = f_backpack.First().GetComponent<Backpack>().available_slots;
		foreach (var item in backp){
			if (item.Item1 == "key" && item.Item2 == lockedDoor.GetComponent<ActivationSlot>().slotID){ // if robot has the key
				return false; // TODO: see if we open the door here?
			}
		}

		configureDialog("HEY ! Tu n'as pas la clé pour ouvrir cette porte.");
		return false;
	}

}