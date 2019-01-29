using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class DynamicPOI : MonoBehaviour
    {
        public float poiSpeed = 0.1f; // base speed of the POI movement
        public float now = 13800000000;

        private Queue<Action> initPlot = new Queue<Action>();
        private List<POITracker> points = new List<POITracker>();

        private PlotPatternSpiralPath pattern;

        private Transform galaxy;

        private float wait = 0;

        public void PlotItems(string context)
        {
            if (context == "GalaxyView")
            {
                Transform hero = transform.Find(context + "Content/SceneLoadHider/HeroView");
                galaxy = hero.Find("POIRotation");

                // get the elements to plot
                ChronozoomLoader loader = new ChronozoomLoader();
                loader.SuperCollection = "chronozoom";

                StartCoroutine(loader.GetChronozoomData((List<Exhibit> exList) =>
                {
                    //PlotPatternGalaxy pattern = new PlotPatternGalaxy();
                    pattern = hero.GetComponent<PlotPatternSpiralPath>();
                    pattern.Setup(exList.Count);

                    Timekeeper timekeeper = GameObject.Find("/ViewLoader").GetComponent<Timekeeper>();

                    int i = 0;

                    // start tracking years after the CZ data has loaded (then start plotting per year)
                    timekeeper.SwitchMode(TimeMode.Galaxy);

                    // setup the timed POI spawning based on the timekeeper
                    timekeeper.updateActions.Add(year =>
                    {
                        float timediff = year - now;
                        List<Exhibit> toPlot = exList.FindAll(ex => ex.time <= timediff);

                        if (toPlot.Count > 0)
                        {
                            toPlot.ForEach((Exhibit ex) =>
                            {
                                exList.Remove(ex);
                                initPlot.Enqueue(() => PlotPOI(
                                    i++,
                                    ex.id,
                                    ex.title,
                                    null,
                                    pattern.GetSpiralNode(i - 1),
                                    pattern.GetSpiralNode(i)));
                            });
                        }
                    });
                }));
            }
        }

        POITracker Jump(POITracker tracker)
        {
            tracker.jump = false;
            tracker.targetIndex = 0;

            if (tracker.gameObject.transform.IsChildOf(pattern.spiralPath.transform))
            {
                // was on spiral, move to path
                tracker.gameObject.transform.SetParent(pattern.bezPath.transform);

                tracker.target = pattern.GetPathNode(0);
                tracker.next = (int i) => pattern.GetPathNode(i);
            }
            else if (tracker.gameObject.transform.IsChildOf(pattern.bezPath.transform))
            {
                // was on path, move to galaxy
                tracker.gameObject.transform.SetParent(galaxy);

                tracker.target = galaxy.Find("POITarget");
                tracker.next = (int i) => null;
            }
            else if (tracker.gameObject.transform.IsChildOf(galaxy))
            {
                // was on galaxy, REMOVE FROM LIST AND DELETE OBJECT
                Destroy(tracker.gameObject);
                return null;
            }

            return tracker;
        }

        // move the POI towards the next node
        POITracker Move(POITracker tracker)
        {
            float step = (poiSpeed * Time.deltaTime) * GalaticController.instance.speedMultiplier;
            if (tracker.gameObject.transform.IsChildOf(galaxy)) step *= .1f; // move slower inside galaxy (it quickly reaches the centre)
            if (tracker.gameObject.transform.IsChildOf(pattern.bezPath.transform)) step *= 1.5f; // appears to move slower on the path, speed up

            tracker.gameObject.transform.position =
                Vector3.MoveTowards(tracker.gameObject.transform.position, tracker.target.position, step);

            return tracker;
        }

        void Update()
        {
            wait += Time.deltaTime;
            if (wait >= .5f)
            {
                wait %= .5f;
                if (initPlot.Count > 0) initPlot.Dequeue()();
                // plot once per .5 sec, so items have some distance apart - sacrifices exact timing
            }

            if (points.Count > 0) // move the points along
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i] != null)
                    {
                        if (points[i].jump) points[i] = Jump(points[i]);
                        GameObject obj = points[i].gameObject;
                        if (obj != null)
                        {
                            Vector3 oldPos = obj.transform.position;

                            points[i] = Move(points[i]);

                            if (oldPos == obj.transform.position)
                            {
                                // poi has reached target, switch to next node and MOVE AGAIN HERE
                                Transform target = points[i].next(++points[i].targetIndex);
                                if (target == null) // no more nodes, jump to next rail
                                {
                                    points[i] = Jump(points[i]);
                                }
                                else
                                {
                                    obj.transform.SetParent(points[i].target);
                                    points[i].target = target;
                                }

                                // point item can be removed after jump, gameobject being null is the indicator
                                if (points[i] != null) points[i] = Move(points[i]);
                            }
                        }
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

            points.Add(new POITracker()
            {
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
