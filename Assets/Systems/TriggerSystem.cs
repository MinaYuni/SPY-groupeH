using UnityEngine;
using FYFY;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class TriggerSystem : FSystem {

	public static TriggerSystem instance;

	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));
	//private Family f_door = FamilyManager.getFamily(new AnyOfTags("LockedDoor"));
	private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));
	private Family f_triggerables = FamilyManager.getFamily(new AllOfComponents(typeof(PopupTriggerable)));

	public GameObject dialogPanel;
	private bool popped;
	private int lastPosX;
	private int lastPosY;

	private GameData gameData;

	public TriggerSystem(){
		instance = this;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){

		GameObject go = GameObject.Find("GameData");
        if (go != null){
            gameData = go.GetComponent<GameData>();
        }

		GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, false);
		popped = false;
	} 

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

		foreach (GameObject player in f_players){	

			Position robotPos = player.GetComponent<Position>();
			int robotDirection = (int) player.GetComponent<Direction>().direction;

			if (popped && (robotPos.x != lastPosX || robotPos.y != lastPosY)){
				popped = false;
			}

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

				// TODO: fix this part so that it can be generic (everything is currently hardcoded for one situation)
				if (triggered){

					switch (triggerable.GetComponent<PopupTriggerable>().popupType){
						case PopupTriggerable.PopupType.Printer:
							if (robotPos.x != lastPosX || robotPos.y != lastPosY){
								configurePopup("Je suis une imprimante 3D. Entrez le code pour la clé que vous souhaitez imprimer.", null, -1, -1, -1, true);
								popped = false;
							} else if (gameData.popupInputText == "8462") {
								configurePopup("Voici votre nouvelle clé. Elle ouvre la porte 01.", "key.png", -1, -1, -1, false);
								gameData.items.Add((triggerablePos.x, triggerablePos.y), ("key", 01));
								gameData.popupInputText = null;
							}
							break;
						case PopupTriggerable.PopupType.LockedDoor:
							triggerLockedDoor(triggerable);
							break;
						case PopupTriggerable.PopupType.Note:
							configurePopup("Il semblerait que quelqu'un a oublié ça ici... C'est écrit : 'Code porte: 8462'", null, -1, -1, -1, false);
							break;
					}
				}
			}
			lastPosX = robotPos.x;
			lastPosY = robotPos.y;
			// Careful: last two lines assume we only have one robot
		}


	}

	/* Checks whether the robot is facing the triggerable.  */
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

	/* Checks whether the robot is in the same cell as the triggerable. */
	private bool playerOnTriggerable(int robot_posX, int robot_posY, int triggerable_posX, int triggerable_posY){
		
		if (robot_posX == triggerable_posX && robot_posY == triggerable_posY){
			return true;
		} 

		return false;
	}

	private void configurePopup(string popupText, string imageName, float imageHeight, int camX, int camY, bool input){

		if (!popped){
			gameData.popup = new List<(string, string, float, int, int, bool)>(1){(popupText, imageName, imageHeight, camX, camY, input)};
			popped = true;
		} 
	}

	/* Triggers popup if the door is locked door and the robot doesn't have the key.  */
	private void triggerLockedDoor(GameObject lockedDoor){

		// Nothing to do if door is open
		if (!lockedDoor.activeSelf) {
			popped = false;
			return;
		}
		
		// If door is locked, check backpack for matching key
		var backp = f_backpack.First().GetComponent<Backpack>().available_slots;
		foreach (var item in backp){
			if (item.Item1 == "key" && item.Item2 == lockedDoor.GetComponent<ActivationSlot>().slotID){ // if robot has the key
				popped = false; // TODO: see if we open the door here?
				return;
			}
		}

		configurePopup("HEY ! Tu n'as pas la clé pour ouvrir cette porte.", null, -1, -1, -1, false);
	}

}