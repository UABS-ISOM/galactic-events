using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class DynamicPOI : MonoBehaviour
    {
        public float poiSpeed = 0.1f;

        private Queue<Action> initPlot = new Queue<Action>();
        private List<POITracker> points = new List<POITracker>();

        private PlotPatternSpiralPath pattern;

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
                    pattern = hero.GetComponent<PlotPatternSpiralPath>();
                    pattern.Setup(exList.Count);

                    int i = 0;


                    //
                    initPlot.Enqueue(() => PlotPOI(
                        i++,
                        exList[0].id,
                        exList[0].title,
                        null,
                        pattern.GetSpiralNode(i - 1),
                        pattern.GetSpiralNode(i)));

                    //exList.ForEach((Exhibit ex) =>
                    //{
                    //    // initPlot.Enqueue(() => PlotPOI(ex.id, ex.title, null, pattern.GetSpiralNode(i++)));
                    //    initPlot.Enqueue(() => PlotPOI(
                    //        i++,
                    //        ex.id,
                    //        ex.title,
                    //        null,
                    //        pattern.GetSpiralNode(i - 1),
                    //        pattern.GetSpiralNode(i)));
                    //    // example: PlotPOI("test", "SOMETHING!", "ChronozoomMenuView", new Vector3(0, 0, 0));
                    //});
                }));
            }
        }

        POITracker Jump (POITracker tracker)
        {
            tracker.jump = false;
            tracker.targetIndex = 0;

            if (tracker.gameObject.transform.IsChildOf(pattern.spiralContainer.transform))
            {
                // was on spiral, move to path
                tracker.gameObject.transform.SetParent(pattern.bezPath.transform);
                
                tracker.target = pattern.GetPathNode(0);
                tracker.next = (int i) => pattern.GetPathNode(i);
            }

            return tracker;
        }

        // move the POI towards the next node
        POITracker Move (POITracker tracker)
        {
            float step = poiSpeed * Time.deltaTime;
            tracker.gameObject.transform.position =
                Vector3.MoveTowards(tracker.gameObject.transform.position, tracker.target.position, step);

            return tracker;
        }

        void Update()
        {
            if (initPlot.Count > 0) initPlot.Dequeue()(); // plot once per update
            if (points.Count > 0) // move the points along
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].jump) points[i] = Jump(points[i]);
                    Vector3 oldPos = points[i].gameObject.transform.position;

                    points[i] = Move(points[i]);

                    if (oldPos == points[i].gameObject.transform.position)
                    {
                        // poi has reached target, switch to next node and MOVE AGAIN HERE
                        Transform target = points[i].next(++points[i].targetIndex);
                        if (target == null) // no more nodes, jump to next rail
                        {
                            points[i] = Jump(points[i]);
                        }
                        else
                        {
                            points[i].gameObject.transform.SetParent(points[i].target);
                            points[i].target = target;
                        }

                        points[i] = Move(points[i]);
                    }
                }
            }
        }

        // plot a point of interest programmatically
        void PlotPOI(int index, string name, string cardText, string transitionScene, Transform currentNode, Transform targetNode)
        {
            // setup structure
            GameObject poi = (GameObject)Instantiate(
                Resources.Load("Prefabs/POI", typeof(GameObject)),
                currentNode.position, Quaternion.identity,
                currentNode);

            poi.name = name;
            //poi.transform.Translate(position, parent.transform);

            poi.transform.Find("ScaleWithDistance target/POI").gameObject.GetComponent<PointOfInterest>().TransitionScene = transitionScene;
            poi.transform.Find("ScaleWithDistance target/POI/ScaleWithDistance target/FaceCamera/Card").gameObject.GetComponent<TextMesh>().text = cardText;

            points.Add(new POITracker() {
                jump = targetNode == null ? true : false, // target == null if at last spiral node
                targetIndex = index + 1,
                target = targetNode,
                gameObject = poi,
                next = (int i) => pattern.GetSpiralNode(i)
            }); // POI's in motion
        }

        // keep track of the POI and where its moving to
        private class POITracker
        {
            public bool jump { get; set; } // needs to jump to next stage (e.g. path, galaxy)
            public Transform target { get; set; }
            public int targetIndex { get; set; }
            public GameObject gameObject { get; set; }
            public Func<int, Transform> next { get; set; }
        }
    }
}
