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
                Transform hero = transform.Find(context + "Content/SceneLoadHider/HeroView");
                Transform parent = hero.Find("POIRotation");

                // get the elements to plot
                ChronozoomLoader loader = new ChronozoomLoader();
                loader.SuperCollection = "chronozoom";

                StartCoroutine(loader.GetChronozoomData((List<Exhibit> exList) =>
                {
                    //PlotPatternGalaxy pattern = new PlotPatternGalaxy();
                    PlotPatternSpiralPath pattern = hero.GetComponent<PlotPatternSpiralPath>();
                    pattern.Setup(exList.Count);

                    int i = 0;
                    exList.ForEach((Exhibit ex) =>
                    {
                        PlotPOI(ex.id, ex.title, null, pattern.GetSpiralNode(i++));

                        //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                        //obj.name = ex.id;
                        //obj.transform.SetParent(parent);
                        //obj.transform.localScale = new Vector3(.04f, .04f, .04f);

                        //obj.transform.position = transform.position;
                        //obj.transform.Translate(pattern.GetPoint(), parent.transform);

                        // example: PlotPOI("test", "SOMETHING!", "ChronozoomMenuView", new Vector3(0, 0, 0));
                    });
                }));
            }
        }

        // plot a point of interest programmatically
        void PlotPOI(string name, string cardText, string transitionScene, Transform node)
        {
            // setup structure
            GameObject poi = (GameObject)Instantiate(
                Resources.Load("Prefabs/POI", typeof(GameObject)),
                node.position, Quaternion.identity,
                node);

            poi.name = name;
            //poi.transform.Translate(position, parent.transform);

            poi.transform.Find("ScaleWithDistance target/POI").gameObject.GetComponent<PointOfInterest>().TransitionScene = transitionScene;
            poi.transform.Find("ScaleWithDistance target/POI/ScaleWithDistance target/FaceCamera/Card").gameObject.GetComponent<TextMesh>().text = cardText;
        }
    }
}
