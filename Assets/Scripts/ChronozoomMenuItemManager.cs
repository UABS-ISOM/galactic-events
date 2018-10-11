using GalaxyExplorer;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is attached to ChronozoomMenuLeft, ChronozoomMenuCentre and ChronozoomMenuRight in ChronozoomMenuView scene. It is responsible for detecting hovers and clicks for the panels.
public class ChronozoomMenuItemManager : GazeSelectionTarget
{
    public PlayableCollection currentCollection { get; set; }

    public override void OnGazeSelect()
    {
        //Changes the colour of the box to give a highlighted hover effect
        gameObject.transform.Find("BackgroundPanel").GetComponent<Renderer>().material.color = new Color32(80, 133, 159, 255);
    }

    public override void OnGazeDeselect()
    {
        //Changes the colour back to original
        gameObject.transform.Find("BackgroundPanel").GetComponent<Renderer>().material.color = new Color32(26, 67, 124, 255);
    }

    public override bool OnTapped()
    {
        //Store name of chosen collection in order to pass this information to the ChronozoomView
        ChronozoomCollectionChoice.UserChosenSuperCollection = currentCollection.SuperCollection;
        Debug.Log("Chosen " + currentCollection.SuperCollection);

        //Load up the next scene
        TransitionManager.Instance.LoadNextScene("ChronozoomView", gameObject);
        return true;
    }
}
