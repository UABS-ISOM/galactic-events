using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class DynamicPOI : MonoBehaviour
    {
        public void PlotItems(string context)
        {
            if (context == "GalaxyView")
            {
                Transform parent = transform.Find(context + "Content/SceneLoadHider/HeroView/POIRotation");

                // get the elements to plot
                ChronozoomLoader loader = new ChronozoomLoader();
                loader.SuperCollection = "chronozoom";

                StartCoroutine(loader.GetChronozoomData((List<Exhibit> exList) =>
                {
                    exList.ForEach((Exhibit ex) =>
                    {
                        // example: PlotPOI("test", "SOMETHING!", "ChronozoomMenuView", new Vector3(0, 0, 0));
                    });
                }));
            }
        }

        // plot a point of interest programmatically
        void PlotPOI(string name, string cardText, string transitionScene, Vector3 position, Transform parent)
        {
            // setup structure
            GameObject poi = (GameObject)Instantiate(
                Resources.Load("Prefabs/POI", typeof(GameObject)),
                position, Quaternion.identity,
                parent);

            poi.name = name;
            poi.transform.Find("ScaleWithDistance target/POI").gameObject.GetComponent<PointOfInterest>().TransitionScene = transitionScene;
            poi.transform.Find("ScaleWithDistance target/POI/ScaleWithDistance target/FaceCamera/Card").gameObject.GetComponent<TextMesh>().text = cardText;
        }
    }
}
