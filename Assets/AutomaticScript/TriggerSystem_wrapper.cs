using UnityEngine;
using FYFY;

public class TriggerSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject dialogPanel;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "dialogPanel", dialogPanel);
	}

}
