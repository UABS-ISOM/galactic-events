using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//This script is attached to the ChronozoomMenuGroup in ChronozoomMenuView scene. It is responsible for displaying the menu panel data.
public class ChronozoomMenuManager : MonoBehaviour {

    private int pageNumber = 1;
    private const int numberOfPanels = 3;
    private List<PlayableCollection> playableCollectionList;

    public Texture defaultMagicWindowTex;

    //Called on load from ChronozoomBoxManager once the data is loaded
    public void Initiate(List<PlayableCollection> collectionList)
    {
        playableCollectionList = collectionList;

        UpdatePanel();
    }

    public void UpdatePanel()
    {
        //Gets current starting index (index of left collection to display)
        int currentStartingIndex = (pageNumber - 1) * numberOfPanels;
        PlayableCollection leftCollection = (playableCollectionList.Count > currentStartingIndex) ? playableCollectionList[currentStartingIndex] : null;
        PlayableCollection centreCollection = (playableCollectionList.Count > currentStartingIndex + 1) ? playableCollectionList[currentStartingIndex + 1] : null;
        PlayableCollection rightCollection = (playableCollectionList.Count > currentStartingIndex + 2) ? playableCollectionList[currentStartingIndex + 2] : null;

        //Update left collection panel
        GameObject leftCollectionText = transform.Find("ChronozoomMenuLeft/CollectionText").gameObject;
        leftCollectionText.GetComponent<Text>().text = (leftCollection != null)?leftCollection.Title:"";
        GameObject leftCollectionMagicWindow = transform.Find("ChronozoomMenuLeft/ChronozoomMagicWindow").gameObject;
        string leftCollectionURL = (leftCollection != null) ? leftCollection.ImageURL : "";
        StartCoroutine(LoadImageOntoMagicWindow(leftCollectionMagicWindow, leftCollectionURL));
        transform.Find("ChronozoomMenuLeft").GetComponent<ChronozoomMenuItemManager>().currentCollection = leftCollection;

        //Update centre collection panel
        GameObject centreCollectionText = transform.Find("ChronozoomMenuCentre/CollectionText").gameObject;
        centreCollectionText.GetComponent<Text>().text = (centreCollection != null) ? centreCollection.Title : "";
        GameObject centreCollectionMagicWindow = transform.Find("ChronozoomMenuCentre/ChronozoomMagicWindow").gameObject;
        string centreCollectionURL = (centreCollection != null) ? centreCollection.ImageURL : "";
        StartCoroutine(LoadImageOntoMagicWindow(centreCollectionMagicWindow, centreCollectionURL));
        transform.Find("ChronozoomMenuCentre").GetComponent<ChronozoomMenuItemManager>().currentCollection = centreCollection;

        //Update right collection panel
        GameObject rightCollectionText = transform.Find("ChronozoomMenuRight/CollectionText").gameObject;
        rightCollectionText.GetComponent<Text>().text = (rightCollection != null) ? rightCollection.Title : "";
        GameObject rightCollectionMagicWindow = transform.Find("ChronozoomMenuRight/ChronozoomMagicWindow").gameObject;
        string rightCollectionURL = (rightCollection != null) ? rightCollection.ImageURL : "";
        StartCoroutine(LoadImageOntoMagicWindow(rightCollectionMagicWindow, rightCollectionURL));
        transform.Find("ChronozoomMenuRight").GetComponent<ChronozoomMenuItemManager>().currentCollection = rightCollection;
    }

    public void Next()
    {
        //Check to see if there is a next page
        if(playableCollectionList.Count > numberOfPanels * pageNumber)
        {

            if(pageNumber == 1)
            {
                //Going from first page to second page, un grey out left arrow
                GameObject leftArrowGameObject = transform.Find("ChronozoomMenuControl/LeftArrow").gameObject;
                leftArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

                leftArrowGameObject.GetComponent<ChronozoomMenuControl>().isActive = true;
            }

            pageNumber++;
            UpdatePanel();

            if(playableCollectionList.Count < numberOfPanels * (pageNumber+1))
            {
                //Last page, grey out right arrow
                GameObject rightArrowGameObject = transform.Find("ChronozoomMenuControl/RightArrow").gameObject;
                rightArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 50);

                rightArrowGameObject.GetComponent<ChronozoomMenuControl>().isActive = false;
            }

            
        }
    }

    public void Previous()
    {
        if(pageNumber > 1)
        {
            if(playableCollectionList.Count < numberOfPanels * (pageNumber + 1))
            {
                //Last page, ungrey out right arrow
                GameObject rightArrowGameObject = transform.Find("ChronozoomMenuControl/RightArrow").gameObject;
                rightArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

                rightArrowGameObject.GetComponent<ChronozoomMenuControl>().isActive = true;
            }

            pageNumber--;
            UpdatePanel();

            if(pageNumber == 1)
            {
                //First page, grey out left arrow
                GameObject leftArrowGameObject = transform.Find("ChronozoomMenuControl/LeftArrow").gameObject;
                leftArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 50);

                leftArrowGameObject.GetComponent<ChronozoomMenuControl>().isActive = false;
            }
        }
    }

    IEnumerator LoadImageOntoMagicWindow(GameObject magicWindow, string imageURL)
    {
        //If no URL is specified, load the default texture onto the magic window
        if (imageURL == null || imageURL.Equals(""))
        {
            magicWindow.GetComponent<MeshRenderer>().materials[0].mainTexture = defaultMagicWindowTex;
            yield break;
        }

        //Download the texture and display it onto the window
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);

        // Causes HoloLens to crash if dataset large
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL);
        yield return uwr.SendWebRequest();
        tex = DownloadHandlerTexture.GetContent(uwr);

        // Deprecated on HoloLens
        //WWW www = new WWW(imageURL);
        //yield return www;
        //www.LoadImageIntoTexture(tex);
        magicWindow.GetComponent<MeshRenderer>().materials[0].mainTexture = tex;
    }
}
