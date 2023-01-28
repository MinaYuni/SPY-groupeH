using UnityEngine;
using FYFY;

public class NewSystem : FSystem {

	public static NewSystem instance;
	private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));
	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));
	private GameData gameData = FamilyManager.getFamily(new AllOfComponents(typeof(GameData))).First().GetComponent<GameData>();
	// private Family itemSlot = FamilyManager.getFamily(new AllOfComponents(typeof(ItemSlot)));
	
	public GameObject backpackPanel;
	public GameObject itemSlot;

	// private ItemSlot itemObj;

	public NewSystem()
	{
		instance = this;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){
		// itemObj = itemSlot.
		// slot = Instantiate(childPrefab, parent.transform)
		int i = 12;
		while (i > 0)
		{
			GameObject newSlot = Object.Instantiate(itemSlot, backpackPanel.transform);
			GameObjectManager.bind(newSlot);
			i = i - 1;
		}
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
	}
}