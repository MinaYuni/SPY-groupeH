using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class GameData : MonoBehaviour {
	// Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
	public GameObject LevelGO;
	public Dictionary<string, XmlNode> levels; // The Associated XmlNode to its path
	public string scenarioName; // name of the scenario (campaign)
	public List<string> scenario; // The scenario to play
	public string levelToLoad; // level index to load in levels dictionary
	public int[] levelToLoadScore; // levelToLoadScore[0] = best score (3 stars) ; levelToLoadScore[1] = medium score (2 stars)
	
	public List<(string, string, float, int, int)> dialogMessage; // list of (dialogText, imageName, imageHeight, camX, camY)
	public List<(string, string, float, int, int, bool)> popup = null; // (dialogText, imageName, imageHeight, camX, camY, hasInput) //TODO: this shouldn't be a list
	public string popupInputText; // Text player typed into the popup input field
	
	public Dictionary<(int, int, int), string> triggerMessage; // <posX, posY, direction>, <popup_text>
	public Dictionary<(int, int, int), (string, int)> triggerDoor; // <posX, posY, direction>, <popup_text, door_pos>

	public Dictionary<(int, int), string> items; // <posX, posy>, <item_name, item_id>
	
	public Dictionary<string, int> actionBlockLimit; // Is block available in library?
	public string scoreKey = "score";
	public int totalStep;
	public int totalActionBlocUsed;
	public int totalExecute;
	public int totalCoin;
	public GameObject actionsHistory; // all actions made in the level, displayed at the end
	public float gameSpeed_default = 1f;
	public float gameSpeed_current = 1f;
	public bool dragDropEnabled = true;
}