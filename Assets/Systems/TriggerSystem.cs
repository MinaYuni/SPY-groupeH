using UnityEngine;
using FYFY;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System;

public class TriggerSystem : FSystem {
	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));
	public static TriggerSystem instance;
	private GameData gameData = FamilyManager.getFamily(new AllOfComponents(typeof(GameData))).First().GetComponent<GameData>();
	private Family f_door = FamilyManager.getFamily(new AnyOfTags("Door"));
	private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));

	
	public GameObject dialogPanel;
	private bool popped;
	// private Tuple curr_pos;

	public TriggerSystem()
	{
		instance = this;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){
		GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, false);
		popped = false;
		// curr_pos = (0,0,0);


	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

		foreach(GameObject player in f_players){	
			var pos = player.GetComponent<Position>();
			var direction = player.GetComponent<Direction>();
			var key = (pos.x, pos.y, (int)direction.direction);
			
			if(gameData.triggerMessage.ContainsKey(key)){				
				configureDialog(gameData.triggerMessage[key]);
				popped = true;
			}

			else if(gameData.triggerDoor.ContainsKey(key)){ // if trigger is for a door
				var slot_id = gameData.triggerDoor[key].Item2;
				bool closed = true;
				bool hasKey = false;

				foreach (GameObject door in f_door){
					if(door.GetComponent<ActivationSlot>().slotID == slot_id){
						closed = door.activeSelf; // check if door is closed or open
						var backp = f_backpack.First().GetComponent<Backpack>().available_slots;
						foreach(var item in backp){
							if(item.Item1 == "key" && item.Item2 == slot_id){
								hasKey = true;
							}
						}
					}
				}
				if(closed && !hasKey){ // if door is closed then pop up
					string te = gameData.triggerDoor[key].Item1;
					configureDialog(te);
					popped = true;
				}
				else{
					popped = false; // probably not necessary
				}
			}
			else{
				popped = false;
			}
		}
	}

	private void configureDialog(string popup){
		// set text
		if(popped == false){
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
}