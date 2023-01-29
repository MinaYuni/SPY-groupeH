using UnityEngine;
using FYFY;

public class ItemCollectSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject backpackPanel;
	public UnityEngine.GameObject level;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "backpackPanel", backpackPanel);
		MainLoop.initAppropriateSystemField (system, "level", level);
	}

}
