using UnityEngine;
using FYFY;

public class ItemSlotSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject backpackPanel;
	public UnityEngine.GameObject itemSlot;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "backpackPanel", backpackPanel);
		MainLoop.initAppropriateSystemField (system, "itemSlot", itemSlot);
	}

}
