using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class DynamicPOI : MonoBehaviour
    {
        private Transform parent_POIRot;
        public void PlotItems(string context)
        {
            if (context == "GalaxyView")
            {
                parent_POIRot = transform.Find(context + "Content/SceneLoadHider/HeroView/POIRotation");

                // get the elements to plot, in this case, just test items
                PlotPOI("test", "something!", "ChronozoomMenuView", new Vector3(-1, 0, .2f));
            }
        }

        // plot a point of interest programmatically
        void PlotPOI(string name, string cardText, string transitionScene, Vector3 position)
        {
            // setup structure
            GameObject poi = (GameObject)Instantiate(
                Resources.Load("Prefabs/POI", typeof(GameObject)),
                position, Quaternion.identity,
                parent_POIRot);

            poi.name = name;
            poi.transform.Find("ScaleWithDistance target/POI").gameObject.GetComponent<PointOfInterest>().TransitionScene = transitionScene;
            poi.transform.Find("ScaleWithDistance target/POI/ScaleWithDistance target/FaceCamera/Card").gameObject.GetComponent<TextMesh>().text = cardText;
        }
    }
}
