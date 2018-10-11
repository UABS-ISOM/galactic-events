using GalaxyExplorer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is attached to either ChronozoomMenuControl/LeftArrow or /RightArrow in ChronozoomMenuView scene. It is responsible for detecting hover and clicks over the arrow buttons.
public class ChronozoomMenuControl : GazeSelectionTarget
{
    public GameObject leftButton;
    public GameObject rightButton;
    public string direction;
    public bool isActive;

    private ChronozoomMenuManager menuManager;

	void Start () {
        //Get instance of ChronozoomMenuManager in order to call Previous/Next functions
        menuManager = transform.parent.parent.GetComponent<ChronozoomMenuManager>();

        //Changes the colour back to original
        if (isActive)
        {
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = new Color32(255, 255, 255, 50);
        }
    }

    public override void OnGazeSelect()
    {
        //Changes the colour of the box to give a highlighted hover effect
        if (isActive)
            GetComponent<Image>().color = new Color32(26, 67, 124, 255);
    }

    public override void OnGazeDeselect()
    {
        //Changes the colour back to original
        if (isActive)
        {
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = new Color32(255, 255, 255, 50);
        }
    }

    public override bool OnTapped()
    {
        //Call the function for the appropriate action
        if (direction.Equals("right") && isActive)
        {
            menuManager.Next();
        } else if (direction.Equals("left") && isActive)
        {
            menuManager.Previous();
        }
        
        return true;
    }
}
