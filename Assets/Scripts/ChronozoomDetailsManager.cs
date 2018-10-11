using GalaxyExplorer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//This script is attached to the ChronozoomBox prefab. It is responsible for displaying data based on appropriate action.
public class ChronozoomDetailsManager : MonoBehaviour {

    public List<ContentItem> contentItems { get; set; }
    private int pageNumber = 1;
    private int numberOfPanels = 2;

    public void Initiate()
    {
        //Display left and right panel for first time load
        DetailsPanel left = (contentItems.Count > 0) ? new DetailsPanel(contentItems[0].title, contentItems[0].description, contentItems[0].uri): new DetailsPanel("", "", "");
        DetailsPanel right = (contentItems.Count > 1) ? new DetailsPanel(contentItems[1].title, contentItems[1].description, contentItems[1].uri) : new DetailsPanel("", "", "");
        DisplayPanelData(left, right);

        UpdatePageDisplay();

        if(contentItems.Count <= numberOfPanels)
        {
            //Gray out right button
            GameObject rightArrowGameObject = transform.Find("DetailData/Canvas/RightImage").gameObject;
            rightArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 50);

            rightArrowGameObject.GetComponent<ChronozoomDetailControl>().isActive = false;
        }
    }

    //Display next page
    public void Next()
    {
        if (pageNumber * numberOfPanels < contentItems.Count)
        {
            int index = pageNumber * numberOfPanels;
            DetailsPanel left = (index <= contentItems.Count) ? new DetailsPanel(contentItems[index].title, contentItems[index].description, contentItems[index].uri) : new DetailsPanel("", "", "");
            DetailsPanel right = (index + 1 < contentItems.Count) ? new DetailsPanel(contentItems[index+1].title, contentItems[index+1].description, contentItems[index+1].uri) : new DetailsPanel("", "", "");
            DisplayPanelData(left, right);
            pageNumber++;

            //Un-gray out left arrow
            GameObject leftArrowGameObject = transform.Find("DetailData/Canvas/LeftImage").gameObject;
            leftArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            leftArrowGameObject.GetComponent<ChronozoomDetailControl>().isActive = true;

            if (pageNumber * numberOfPanels >= contentItems.Count)
            {
                //Last page. Need to gray out right arrow
                GameObject rightArrowGameObject = transform.Find("DetailData/Canvas/RightImage").gameObject;
                rightArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 50);

                rightArrowGameObject.GetComponent<ChronozoomDetailControl>().isActive = false;
            }

            UpdatePageDisplay();
        }
    }

    //Display previous page
    public void Previous()
    {
        if (pageNumber > 1)
        {
            pageNumber--;
            int index = pageNumber * numberOfPanels - numberOfPanels;
            DetailsPanel left = (index <= contentItems.Count) ? new DetailsPanel(contentItems[index].title, contentItems[index].description, contentItems[index].uri) : new DetailsPanel("", "", "");
            DetailsPanel right = (index + 1 <= contentItems.Count) ? new DetailsPanel(contentItems[index + 1].title, contentItems[index + 1].description, contentItems[index + 1].uri) : new DetailsPanel("", "", "");
            DisplayPanelData(left, right);

            //Un-gray out right arrow
            GameObject rightArrowGameObject = transform.Find("DetailData/Canvas/RightImage").gameObject;
            rightArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            rightArrowGameObject.GetComponent<ChronozoomDetailControl>().isActive = true;

            if (pageNumber == 1)
            {
                //First page. Need to gray out left arrow
                GameObject leftArrowGameObject = transform.Find("DetailData/Canvas/LeftImage").gameObject;
                leftArrowGameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 50);

                leftArrowGameObject.GetComponent<ChronozoomDetailControl>().isActive = false;
            }

            UpdatePageDisplay();
        }
        
    }

    private void UpdatePageDisplay()
    {
        GameObject pageDisplayText = transform.Find("DetailData/Canvas/PageDisplay").gameObject;
        pageDisplayText.GetComponent<Text>().text = pageNumber + " / " + ((contentItems.Count + numberOfPanels -1)/numberOfPanels);
    }

    private void DisplayPanelData(DetailsPanel left, DetailsPanel right)
    {
        //Finds the left detail panel and change heading with chronozoom data
        GameObject leftDetailHeadingText = this.transform.Find("DetailData/InfoBackPanelLeft/Canvas/leftDetailHeading").gameObject;
        leftDetailHeadingText.GetComponent<Text>().text = left.HeadingText;

        //Finds the left detail panel and change content with chronozoom data. 
        GameObject leftDetailText = this.transform.Find("DetailData/InfoBackPanelLeft/Canvas/leftDetailDescription").gameObject;
        leftDetailText.GetComponent<Text>().text = left.ContentText;

        //Finds the right detail panel and change heading with chronozoom data
        GameObject rightDetailHeadingText = this.transform.Find("DetailData/InfoBackPanelRight/Canvas/rightDetailHeading").gameObject;
        rightDetailHeadingText.GetComponent<Text>().text = right.HeadingText;

        //Finds the right detail panel and change content with chronozoom data.
        GameObject rightDetailText = this.transform.Find("DetailData/InfoBackPanelRight/Canvas/rightDetailDescription").gameObject;
        rightDetailText.GetComponent<Text>().text = right.ContentText;

        //Load up image onto magic window
        GameObject leftMagicWindow = this.transform.Find("DetailData/InfoBackPanelLeft/ChronozoomMagicWindow").gameObject;
        String imageURLLeft = left.ImageURL;
        StartCoroutine(LoadImageOntoMagicWindow(leftMagicWindow, imageURLLeft));

        GameObject rightMagicWindow = this.transform.Find("DetailData/InfoBackPanelRight/ChronozoomMagicWindow").gameObject;
        String imageURLRight = right.ImageURL;
        StartCoroutine(LoadImageOntoMagicWindow(rightMagicWindow, imageURLRight));
    }

    IEnumerator LoadImageOntoMagicWindow(GameObject magicWindow, string imageURL)
    {
        if (imageURL == null || imageURL.Equals(""))
        {
            yield break;
        }
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);

        // Causes HoloLens to crash if dataset large
        //UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL);
        //yield return uwr.SendWebRequest();
        //tex = DownloadHandlerTexture.GetContent(uwr);

        // This code does not work on HoloLens and WWW is deprecated
        WWW www = new WWW(imageURL);
        yield return www;
        www.LoadImageIntoTexture(tex);

        magicWindow.GetComponent<MeshRenderer>().materials[0].mainTexture = tex;
    }
}

//Class that holds the heading, content and url
public class DetailsPanel
{
    public string HeadingText { get; set; }
    public string ContentText { get; set; }
    public string ImageURL { get; set; }
    public DetailsPanel(string heading, string content, string image)
    {
        HeadingText = heading;
        ContentText = content;
        ImageURL = image;
    }
}
