using UnityEngine;
using FYFY;

public class ItemCollectSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject backpackPanel;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "backpackPanel", backpackPanel);
	}

}
