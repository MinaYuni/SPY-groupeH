using UnityEngine;
using FYFY;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Diagnostics;

public class DocumentationSystem : FSystem
{
    public static DocumentationSystem instance;

    private Family f_playingMode = FamilyManager.getFamily(new AllOfComponents(typeof(PlayMode)));
    private Family f_editingMode = FamilyManager.getFamily(new AllOfComponents(typeof(EditMode)));

    private GameData gameData;
    public GameObject docPanel;
    public GameObject actionPanel;
    public GameObject controlPanel;
    public GameObject operatorPanel;
    public GameObject captorPanel;

    public DocumentationSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        GameObject go = GameObject.Find("GameData");
        if (go != null)
        {
            gameData = go.GetComponent<GameData>();
        }
    }

    public void openDocPanel()
    {
        if (docPanel.activeSelf == false)
        {
            GameObjectManager.setGameObjectState(docPanel, true);

            //Debug.Log(GBL_Interface.playerName + " asks to send statement...");
            GameObjectManager.addComponent<ActionPerformedForLRS>(MainLoop.instance.gameObject, new
            {
                verb = "opened",
                objectType = "documentation"
            });

        }
        else
        {
            GameObjectManager.setGameObjectState(docPanel, false);
        }
    }

    public void afficheDoc()
    {
        string path = "./Assets/StreamingAssets/doc.xml";
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        // récupérer tous les enfants actifs des blocs d'action 
        Transform[] childrenAction = actionPanel.transform.GetComponentsInChildren<Transform>(false);
        Transform[] childrenControl = controlPanel.transform.GetComponentsInChildren<Transform>(false);
        Transform[] childrenOperator = operatorPanel.transform.GetComponentsInChildren<Transform>(false);
        Transform[] childrenCaptor = captorPanel.transform.GetComponentsInChildren<Transform>(false);
        List<string> allChildrenName = new List<string>();

        // récupérer la partie texte du Doc Panel 
        GameObject textGO = docPanel.transform.Find("DocPanel").Find("Text").gameObject;
        textGO.GetComponent<TextMeshProUGUI>().text = "";

        // récupérer que les enfants direct d'Action Panel 
        foreach (Transform child in childrenAction)
        {
            if (child.parent.name == actionPanel.name)
            {
                //textGO.GetComponent<TextMeshProUGUI>().text += child.name + "\n";
                allChildrenName.Add(child.name);
            }
        }

        // récupérer que les enfants direct de Control Panel 
        foreach (Transform child in childrenControl)
        {
            if (child.parent.name == controlPanel.name)
            {
                //textGO.GetComponent<TextMeshProUGUI>().text += child.name + "\n";
                allChildrenName.Add(child.name);
            }
        }

        // récupérer que les enfants direct de Operator Panel 
        foreach (Transform child in childrenOperator)
        {
            if (child.parent.name == operatorPanel.name)
            {
                //textGO.GetComponent<TextMeshProUGUI>().text += child.name + "\n";
                allChildrenName.Add(child.name);
            }
        }

        // récupérer que les enfants direct de Captor Panel 
        foreach (Transform child in childrenCaptor)
        {
            if (child.parent.name == captorPanel.name)
            {
                //textGO.GetComponent<TextMeshProUGUI>().text += child.name + "\n";
                allChildrenName.Add(child.name);
            }
        }

        if (docPanel.activeSelf == false)
        {
            textGO.GetComponent<TextMeshProUGUI>().text += "DOCUMENTATION \n\n";
            foreach (string childName in allChildrenName)
            {
                XmlNode node = doc.SelectSingleNode("//blockLimit[@blockType='" + childName + "']");
                textGO.GetComponent<TextMeshProUGUI>().text += childName + " : " + node.InnerText + "\n\n";
            }
        }
        else
        {
            textGO.GetComponent<TextMeshProUGUI>().text = "";
        }

    }
}