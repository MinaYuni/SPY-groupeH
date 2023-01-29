using UnityEngine;
using FYFY;
using UnityEngine.UI;

public class ItemSlotSystem : FSystem {

    public static ItemSlotSystem instance;
    private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));
    private Family f_items = FamilyManager.getFamily(new AllOfComponents(typeof(Item)));
    // private GameData gameData = FamilyManager.getFamily(new AllOfComponents(typeof(GameData))).First().GetComponent<GameData>();
    // private Family itemSlot = FamilyManager.getFamily(new AllOfComponents(typeof(ItemSlot)));
	
    public GameObject backpackPanel;
    // public GameObject itemSlot;
    // hardcode all the items in unity
    // when picked up -> find gameobj w name => enable the image
    // in the onstart, enable already backpack
    

    // private ItemSlot itemObj;

    public ItemSlotSystem()
    {
        instance = this;
    }
	
    // Use to init system before the first onProcess call
    protected override void onStart()
    {
        // check if some items are already in the backpack
        var backp = f_backpack.First().GetComponent<Backpack>().available_slots;
        foreach (var item in backp)
        {
            // Debug.Log("item number 1 : " + item);
            showItemFromStart(item.Item1);
        }

        // now check each time an item is added        
        f_items.addEntryCallback(showItem);
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        
    }

    private void showItem(GameObject itemGo)
    {
        Item item = itemGo.GetComponent<Item>();
        string name = item.id.Item1;
        // Debug.Log("item name : " + name);
        GameObject itemToShow = backpackPanel.transform.Find("key").gameObject;
        // Debug.Log("itemToShow : " + itemToShow);
        GameObject iconGo = itemToShow.transform.Find("IconImage").gameObject;
        
        
        Image image = iconGo.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = true;
        }

    }
    
    private void showItemFromStart(string name)
    {
        // Debug.Log("item name : " + name);
        GameObject itemToShow = backpackPanel.transform.Find("key").gameObject;
        // Debug.Log("itemToShow : " + itemToShow);
        GameObject iconGo = itemToShow.transform.Find("IconImage").gameObject;
        
        
        Image image = iconGo.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = true;
        }
        // Debug.Log("child : " + itemToShow.transform.GetChild(1).gameObject);
        // Debug.Log("child is active : " + itemToShow.transform.GetChild(1).gameObject.activeSelf);


    }
}