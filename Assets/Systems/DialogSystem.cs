using UnityEngine;
using FYFY;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System;

/// <summary>
/// Manage dialogs at the begining of the level
/// </summary>
public class DialogSystem : FSystem
{
    private Family f_playingMode = FamilyManager.getFamily(new AllOfComponents(typeof(PlayMode)));
    private Family f_editingMode = FamilyManager.getFamily(new AllOfComponents(typeof(EditMode)));

    private GameData gameData;
    public GameObject dialogPanel;
    public GameObject showDialogsMenu;
    public GameObject showDialogsBottom;
    private int nDialog = 0;

    protected override void onStart()
    {
        GameObject go = GameObject.Find("GameData");
        if (go != null)
        {
            gameData = go.GetComponent<GameData>();

            // Always disable bottom button, it will be enabled at the end of the dialogs (see Ok button)
            GameObjectManager.setGameObjectState(showDialogsBottom, false);
            if (gameData.dialogMessage.Count == 0)
                showDialogsMenu.GetComponent<Button>().interactable = false;
            else
                showDialogsMenu.GetComponent<Button>().interactable = true;
        }

        f_playingMode.addEntryCallback(delegate
        {
            GameObjectManager.setGameObjectState(showDialogsBottom, false);
        });

        f_editingMode.addEntryCallback(delegate
        {
            if (gameData.dialogMessage.Count > 0)
                GameObjectManager.setGameObjectState(showDialogsBottom, true);
        });

        GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, false);
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        //Activate DialogPanel if there is a message
        if (gameData != null && nDialog < gameData.dialogMessage.Count && !dialogPanel.transform.parent.gameObject.activeSelf){
            showDialogPanel();
        } else if (gameData != null && gameData.popup != null){
        	showPopup();
        }
    }


    // Affiche le panneau de dialoge au début de niveau (si besoin)
    public void showDialogPanel()
    {
        GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, true);
        nDialog = 0;

        Debug.Log(GBL_Interface.playerName + " asks to send statement...");
        GameObjectManager.addComponent<ActionPerformedForLRS>(gameData.LevelGO, new
        {
            verb = "opened",
            objectType = "instructions"
        });

        configureDialog();
    }

    public void showPopup(){
    	
    	GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, true);
        configurePopup();
    }

    // See NextButton in editor
    // Permet d'afficher la suite du dialogue
    public void nextDialog()
    {
        nDialog++; // On incrémente le nombre de dialogue

        configureDialog();
    }

    // See PreviousButton in editor
    // Permet d'afficher le message précédent
    public void prevDialog()
    {
        nDialog--; // On décrémente le nombre de dialogue

        configureDialog();
    }

    private void configureDialog()
    {
        // set text
        GameObject textGO = dialogPanel.transform.Find("Text").gameObject;
        if (gameData.dialogMessage[nDialog].Item1 != null)
        {
            GameObjectManager.setGameObjectState(textGO, true);
            textGO.GetComponent<TextMeshProUGUI>().text = gameData.dialogMessage[nDialog].Item1;
            LayoutRebuilder.ForceRebuildLayoutImmediate(textGO.transform as RectTransform);
        }
        else
            GameObjectManager.setGameObjectState(textGO, false);
        // set image
        GameObject imageGO = dialogPanel.transform.Find("Image").gameObject;
        if (gameData.dialogMessage[nDialog].Item2 != null)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Uri uri = new Uri(gameData.levelToLoad);
                setImageSprite(imageGO.GetComponent<Image>(), gameData.dialogMessage[nDialog].Item3, uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments[uri.Segments.Length - 1].Length) + "Images/" + gameData.dialogMessage[nDialog].Item2);
            }
            else
                setImageSprite(imageGO.GetComponent<Image>(), gameData.dialogMessage[nDialog].Item3, Path.GetDirectoryName(gameData.levelToLoad) + "/Images/" + gameData.dialogMessage[nDialog].Item2);
        }
        else
            GameObjectManager.setGameObjectState(imageGO, false);
        // set camera pos
        if (gameData.dialogMessage[nDialog].Item4 != -1 && gameData.dialogMessage[nDialog].Item5 != -1)
        {
            GameObjectManager.addComponent<FocusCamOn>(MainLoop.instance.gameObject, new { camX = gameData.dialogMessage[nDialog].Item4, camY = gameData.dialogMessage[nDialog].Item5 });
        }

        // Be sure all buttons are disabled
        setActiveOKButton(false);
        setActiveNextButton(false);
        setActivePrevButton(false);

        if (nDialog + 1 < gameData.dialogMessage.Count)
            setActiveNextButton(true);
        if (nDialog > 0)
            setActivePrevButton(true);
        if (nDialog + 1 >= gameData.dialogMessage.Count)
            setActiveOKButton(true);
    }

    private void configurePopup(){
        
    	string popupText = gameData.popup[0].Item1;
    	string imageName = gameData.popup[0].Item2;
    	float imageHeight  = gameData.popup[0].Item3;
    	int camX = gameData.popup[0].Item4;
    	int camY = gameData.popup[0].Item5;
    	bool hasInput = gameData.popup[0].Item6;

    	// Set text
		GameObject textGO = dialogPanel.transform.Find("Text").gameObject;
		GameObjectManager.setGameObjectState(textGO, true);
		textGO.GetComponent<TextMeshProUGUI>().text = popupText;

		// Set image
        GameObject imageGO = dialogPanel.transform.Find("Image").gameObject;
        if (imageName != null){
            if (Application.platform == RuntimePlatform.WebGLPlayer){
                Uri uri = new Uri(gameData.levelToLoad);
                setImageSprite(imageGO.GetComponent<Image>(), imageHeight, uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments[uri.Segments.Length - 1].Length) + "Images/" + imageName);
            } else {
                setImageSprite(imageGO.GetComponent<Image>(), imageHeight, Path.GetDirectoryName(gameData.levelToLoad) + "/Images/" + imageName);
            }
        } else {
            GameObjectManager.setGameObjectState(imageGO, false);
        }

		// Set input field
		GameObject inputGO = dialogPanel.transform.Find("InputField").gameObject;
		if (hasInput){
			GameObjectManager.setGameObjectState(inputGO, true);
		} else {
			GameObjectManager.setGameObjectState(inputGO, false);
		}

		// Set camera pos
        if (camX != -1 && camY != -1){
            GameObjectManager.addComponent<FocusCamOn>(MainLoop.instance.gameObject, new { camX = camX, camY = camY });
        }

		LayoutRebuilder.ForceRebuildLayoutImmediate(textGO.transform as RectTransform);

		setActiveOKButton(true);
		setActiveNextButton(false);
    	setActivePrevButton(false);
    }


    // Active ou non le bouton Ok du panel dialogue
    public void setActiveOKButton(bool active)
    {
        GameObjectManager.setGameObjectState(dialogPanel.transform.Find("Buttons").Find("OKButton").gameObject, active);
    }


    // Active ou non le bouton next du panel dialogue
    public void setActiveNextButton(bool active)
    {
        GameObjectManager.setGameObjectState(dialogPanel.transform.Find("Buttons").Find("NextButton").gameObject, active);
    }


    // Active ou non le bouton next du panel dialogue
    public void setActivePrevButton(bool active)
    {
        dialogPanel.transform.Find("Buttons").Find("PrevButton").gameObject.GetComponent<Button>().interactable = active;
    }

    // See OKButton in editor
    // Désactive le panel de dialogue
    public void closeDialogPanel()
    {
        GameObjectManager.setGameObjectState(dialogPanel.transform.parent.gameObject, false);
        if (gameData.popup == null){ // Level dialog
            nDialog = gameData.dialogMessage.Count;
        } else { // Popup
            GameObject inputGO = dialogPanel.transform.Find("InputField").gameObject;
            gameData.popupInputText = inputGO.GetComponent<TMP_InputField>().text;
            inputGO.GetComponent<TMP_InputField>().text = null;
            gameData.popup = null;
        }
    }

    // Affiche l'image associée au dialogue
    public void setImageSprite(Image img, float imageHeight, string path)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            MainLoop.instance.StartCoroutine(GetTextureWebRequest(img, imageHeight, path));
        }
        else
        {
            Texture2D tex2D = new Texture2D(2, 2); //create new "empty" texture
            try
            {
                byte[] fileData = File.ReadAllBytes(path); //load image from SPY/path
                if (tex2D.LoadImage(fileData))
                {
                    img.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0, 0), 100.0f);
                    MainLoop.instance.StartCoroutine(setWantedHeight(img, imageHeight));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private IEnumerator GetTextureWebRequest(Image img, float imageHeight, string path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
            img.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0, 0), 100.0f);
            MainLoop.instance.StartCoroutine(setWantedHeight(img, imageHeight));
        }
    }

    private IEnumerator setWantedHeight(Image img, float imageHeight)
    {
        img.gameObject.SetActive(false); // Force disabling image to compute panel height with only buttons and text
        yield return new WaitForSeconds(0.1f); // take time to update UI
        RectTransform rectParent = (RectTransform)img.transform.parent;
        float maxHeight = Screen.height - (rectParent.sizeDelta.y + rectParent.anchoredPosition.y * 2); // compute available space
        if (imageHeight != -1)
            ((RectTransform)img.transform).sizeDelta = new Vector2(((RectTransform)img.transform).sizeDelta.x, Math.Min(gameData.dialogMessage[nDialog].Item3, maxHeight));
        else
            ((RectTransform)img.transform).sizeDelta = new Vector2(((RectTransform)img.transform).sizeDelta.x, Math.Min(img.GetComponent<LayoutElement>().preferredHeight, maxHeight));
        GameObjectManager.setGameObjectState(img.gameObject, true); // Know we can show image
    }
}