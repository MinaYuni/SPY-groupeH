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
		// Debug.Log("hey we are in the onProcess");
		foreach(GameObject player in f_players){
			
			var pos = player.GetComponent<Position>();
			var direction = player.GetComponent<Direction>();
			var key = (pos.x, pos.y, (int)direction.direction);
			
			if(gameData.triggerMessage.ContainsKey(key)){
				// Debug.Log(gameData.triggerMessage[key]);
				
				configureDialog(gameData.triggerMessage[key]);
				// popped = true;

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