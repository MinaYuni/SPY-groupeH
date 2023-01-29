using UnityEngine;
using FYFY;

public class DocumentationSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject docPanel;
	public UnityEngine.GameObject actionPanel;
	public UnityEngine.GameObject controlPanel;
	public UnityEngine.GameObject operatorPanel;
	public UnityEngine.GameObject captorPanel;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "docPanel", docPanel);
		MainLoop.initAppropriateSystemField (system, "actionPanel", actionPanel);
		MainLoop.initAppropriateSystemField (system, "controlPanel", controlPanel);
		MainLoop.initAppropriateSystemField (system, "operatorPanel", operatorPanel);
		MainLoop.initAppropriateSystemField (system, "captorPanel", captorPanel);
	}

	public void openDocPanel()
	{
		MainLoop.callAppropriateSystemMethod (system, "openDocPanel", null);
	}

	public void afficheDoc()
	{
		MainLoop.callAppropriateSystemMethod (system, "afficheDoc", null);
	}

}
