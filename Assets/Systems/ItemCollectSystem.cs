using UnityEngine;
using FYFY;

public class ItemCollectSystem : FSystem {

	// = controller to add to the backpack and to the overlay in unity

	public static ItemCollectSystem instance;
	private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));
	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));

	private GameData gameData = FamilyManager.getFamily(new AllOfComponents(typeof(GameData))).First().GetComponent<GameData>();

	public GameObject backpackPanel;
	public GameObject level;
	// private Vector2 pos = new Vector2(1, 2);
	// private bool collected = false;
	//add a family of items on the level??

	public ItemCollectSystem()
	{ 
		instance = this;
		
	}
// deux component = pos and type/name/id... 
	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		
		foreach(var player in f_players){

			var p_pos = player.GetComponent<Position>();
			var key = (p_pos.x, p_pos.y);
			// Debug.Log(key + " player pos");

			if(gameData.items.ContainsKey(key))
			{
				string name = gameData.items[key];
				Debug.Log("name : " + name);

				f_backpack.First().GetComponent<Backpack>().available_slots.Add(gameData.items[key]);
				Debug.Log("item just now added : "+ gameData.items[key]);
				
				// Item it = Item();
				// it.id = gameData.items[key]; 
				GameObjectManager.addComponent<Item>(backpackPanel, new{id=gameData.items[key]});
				gameData.items.Remove(key); // item has been picked up

				
				Debug.Log("backpack : " + f_backpack.First().GetComponent<Backpack>().available_slots[0]);
				// Debug.Log("backpack : " + f_backpack.First().GetComponent<Backpack>().available_slots[1]);
				//
				if (level.transform.Find(name + "(Clone)").gameObject != null)
				{
					GameObject itemToHide = level.transform.Find(name + "(Clone)").gameObject;
					Debug.Log(itemToHide);
					
					itemToHide.SetActive(false);
				}
				// Debug.Log("gameData items before removing : " + gameData.items);
				// foreach(var i in gameData.items)
				// {
				// 	Debug.Log("item in gamedata before removing : " + i);
				// }
				// gameData.items.Remove(key); // item has been picked up
				// // Debug.Log("gameData items : " + gameData.items);
				// foreach(var i in gameData.items)
				// {
				// 	Debug.Log("gamedata item : " + i);
				// }

				// gameData.items.Remove(((int)pos.x, (int)pos.y)); // item has been picked up

			}
		}
	}


}