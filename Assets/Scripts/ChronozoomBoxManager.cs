using GalaxyExplorer;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using UnityEngine;

//This script is attached to the ChronozoomBox prefab that becomes instantiated programmatically. It handles the hovers and clicks for the box.
public class ChronozoomBoxManager : GazeSelectionTarget
{
    public static ChronozoomBoxManager ActiveBox;
    private ChronozoomPresentToPlayer present;

    public void Start()
    {
        // Turn off our animator until it's needed
        GetComponent<Animator>().enabled = false;
        present = GetComponent<ChronozoomPresentToPlayer>();
    }


    public override void OnGazeSelect()
    {
        //Changes the colour of the box to give a highlighted hover effect
        gameObject.transform.Find("PanelFront").GetComponent<Renderer>().material.color = new Color32(143, 87, 201,255) ;
    }

    public override void OnGazeDeselect()
    {
        //Changes the colour back to original
        gameObject.transform.Find("PanelFront").GetComponent<Renderer>().material.color = new Color32(120, 36, 206,255);
    }

    public override bool OnTapped()
    {
        //Updates the current active exhibit to the current one
        present = gameObject.GetComponent<ChronozoomPresentToPlayer>();
        if (ChronozoomPresentToPlayer.ActiveExhibit == present)
        {
            ChronozoomPresentToPlayer.ActiveExhibit = null;
        }
        else
        {
            ChronozoomPresentToPlayer.ActiveExhibit = present;
            if (present.Presenting)
                return true;

            StartCoroutine(UpdateActive());
        }

        return true;
    }

    public IEnumerator UpdateActive()
    {
        present.Present();

        while (!present.InPosition)
        {
            // Wait for the item to be in presentation distance before animating
            yield return null;
        }

        // Start the animation
        Animator animator = gameObject.GetComponent<Animator>();
        animator.enabled = true;
        animator.SetBool("Opened", true);

        while (ChronozoomPresentToPlayer.ActiveExhibit == present)
        {
            // Wait for the player to send it back
            yield return null;
        }

        animator.SetBool("Opened", false);

        yield return new WaitForSeconds(0.66f);      

        // Return the item to its original position
        present.Return();
    }
}
