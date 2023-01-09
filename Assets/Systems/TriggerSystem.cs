using UnityEngine;
using FYFY;

public class TriggerSystem : FSystem {
	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));

	public static TriggerSystem instance;
// fam manager to get agent edit then allCompononents
// 
	private GameData gameData = FamilyManager.getFamily(new AllOfComponents(typeof(GameData))).First().GetComponent<GameData>();

	public TriggerSystem()
	{
		instance = this;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){

	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		Debug.Log("hey we are in the onProcess");
		foreach(GameObject go in f_players){
			
			var pos = go.GetComponent<Position>();
			var direction = go.GetComponent<Direction>();
			var key = (pos.x, pos.y, (int)direction.direction);
			
			if(gameData.triggerMessage.ContainsKey(key)){
				Debug.Log(gameData.triggerMessage[key]);

			}
			
			
		}
	}
}