using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace GalaxyExplorer
{
    //This script is attached to Content in ChronozoomView scene. It is responsible for loading the chronozoom data from chronozoom.com
    public class ChronozoomLoader : MonoBehaviour
    {
        public GameObject panelBox;
        public GameObject detailsCanvas;

        private const string ChronozoomURI = "http://www.chronozoom.com/api/gettimelines?supercollection=";
        private string SuperCollection = ChronozoomCollectionChoice.UserChosenSuperCollection;
        private List<Exhibit> exhibitList;
        private bool onlyPictures = true;

        //Change this to limit the number of exhibits that are instantiated. Set limit to around 100 to prevent text overload
        private int maxExhibits = 100;

        void Awake()
        {
            //Called on scene load
            if (SuperCollection == null || SuperCollection.Equals(""))
            {
                SuperCollection = "cosmos";
            }
            StartCoroutine(GetChronozoomData());
        }

        IEnumerator GetChronozoomData()
        {
            Debug.Log("Getting Chronozoom Data");
            UnityWebRequest www = UnityWebRequest.Get(ChronozoomURI + SuperCollection);
            // www.timeout = 300;
            yield return www.SendWebRequest();
            // KNown Error https://issuetracker.unity3d.com/issues/wsa-isnetworkerror-always-return-true-when-running-an-uwp-app-on-86x-architecture
            //if (www.isNetworkError) // This fails only on HoloLens where it used to work
            //{
            //    Debug.Log(www.error);
            //    yield return new WaitForSeconds(2); // Why this?
            //    DeserializeData(www.downloadHandler.text);
            //}
            //else
            {
                Debug.Log("Retrieved Chronozoom Data For " + SuperCollection);
                yield return new WaitForSeconds(2);
                DeserializeData(www.downloadHandler.text);
            }
        }

        void DeserializeData(string data)
        {
            //Convert json into Timeline objects
            Timeline timeline = new Timeline();
            timeline = JsonConvert.DeserializeObject<Timeline>(data);
            DisplayData(timeline);
        }

        void DisplayData(Timeline timeline)
        {
            float xOffSet = 0.0533f;
            float xPosition = 0f;
            int exhibitCount = 0;

            var panelBoxGroup = new GameObject();
            panelBoxGroup.transform.parent = GameObject.Find("ChronozoomContent").transform;
            panelBoxGroup.transform.localPosition = Vector3.zero;
            panelBoxGroup.transform.rotation = Quaternion.identity;

            //Retrieve all exhibits from subtimelines
            GetExhibitList(timeline);

            //Sort the exhibits in order of year
            SortExhibit();

            foreach (Exhibit exhibit in exhibitList)
            {
                //Limit number of exhibits by the value speficied in maxExhibit
                if(exhibitCount >= maxExhibits)
                {
                    break;
                }
                exhibitCount++;

                //Instantiate the panel box for displaying information
                GameObject panelBoxGameObject = Instantiate(panelBox);

                panelBoxGameObject.transform.parent = panelBoxGroup.transform;
                Vector3 currentPosition = panelBoxGameObject.transform.position;
                panelBoxGameObject.transform.localPosition = new Vector3(currentPosition.x + xPosition, currentPosition.y, currentPosition.z);

                //Finds the heading text inside the box and change the title with chronozoom data
                GameObject headingText = panelBoxGameObject.transform.Find("Canvas/Heading").gameObject;
                headingText.GetComponent<Text>().text = exhibit.title;

                //Finds the content text inside the box and change the content with chronozoom data
                GameObject yearText = panelBoxGameObject.transform.Find("Canvas/Year").gameObject;
                yearText.GetComponent<Text>().text = String.Format("{0:0,0}", exhibit.time);

                //Finds the collection text inside the box and change the content with chronozoom data
                GameObject collectionText = panelBoxGameObject.transform.Find("Canvas/Collection").gameObject;
                collectionText.GetComponent<Text>().text = timeline.Regime;

                //Store exhibits for that panel
                ChronozoomDetailsManager detailsManager = panelBoxGameObject.transform.GetComponent<ChronozoomDetailsManager>();
                detailsManager.contentItems = exhibit.contentItems;
                detailsManager.Initiate();

                //Move position for next box to the right
                xPosition = xOffSet + xPosition;


            }

            //Makes sure the position, rotation and scale stays constant on each load
            var positionCube = GameObject.Find("ChronozoomPositionCube").transform;
            panelBoxGroup.transform.SetPositionAndRotation(positionCube.position, positionCube.rotation);
            panelBoxGroup.transform.localScale = new Vector3(1, 1, 1);

        }

        private void GetExhibitList(Timeline timeline)
        {
            exhibitList = new List<Exhibit>();

            //Recursively go through all the subtimelines
            GetSubTimelinesInTimeline(timeline);

            return;
        }

        private void GetSubTimelinesInTimeline(Timeline timeline)
        {
            //if there are sub timelines in the given timeline drill down and call getExhibits at each level
            if (timeline.timelines != null)
            {
                foreach (Timeline subTimeline in timeline.timelines)
                {
                    //recursive function call to drill down
                    GetSubTimelinesInTimeline(subTimeline);
                    GetExhibitsInTimeline(subTimeline);
                }
            }
            else //if not just get Exhibits
            {
                GetExhibitsInTimeline(timeline);
            }
        }

        //Function to drill down into exhibits to get content items
        private void GetExhibitsInTimeline(Timeline timeline)
        {
            foreach (Exhibit exhibit in timeline.exhibits)
            {
                //In the chronozoom suppercollection there are duplicate exhibits so this just checks for any duplicates before adding - won't not be needed for custom data sets
                bool alreadyExists = exhibitList.Any(item => item.id == exhibit.id);
                if (!alreadyExists)
                {
                    List<ContentItem> contentItemList = new List<ContentItem>();
                    foreach (ContentItem contentItem in exhibit.contentItems)
                    {
                        //Types in Chronozoom include: picture, image, photosynth and video however there is no naming conventions in place when it comes to defining the media source
                        //Based on the setting 'onlyPictures' it either returns all content items or filters to only images
                        bool isValid = ValidateMediaSource(contentItem.uri);
                        bool descriptionValid = !contentItem.description.Equals("") && contentItem.description != null;
                        if (onlyPictures && (contentItem.mediaType.ToUpper() == "PICTURE" || contentItem.mediaType.ToUpper() == "IMAGE") && isValid && descriptionValid)
                        {
                            contentItemList.Add(contentItem);
                        }
                        else if (!onlyPictures)
                        {
                            contentItemList.Add(contentItem);
                        }
                    }
                    exhibit.contentItems = contentItemList;
                    exhibitList.Add(exhibit);
                }
            }
        }

        private void SortExhibit()
        {
            //Sort based on ascending order of time
            exhibitList.Sort((a, b) => a.time.CompareTo(b.time));
        }

        //Checks to see if the media source is valid. Same code from chronoplay
        private static bool ValidateMediaSource(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return false;
            }
            else
            {
                if (4 >= str.Length)
                {
                    return false;
                }
                else if (str.Substring(str.Length - 4).ToUpper() == ".GIF")
                {
                    return false;
                }
                else if (str.ToUpper().IndexOf("PHOTOSYNTH") > -1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }

    public class Timeline
    {
        public string __type { get; set; }
        public bool FromIsCirca { get; set; }
        public string Height { get; set; }
        public string Regime { get; set; }
        public bool ToIsCirca { get; set; }
        public float? aspectRatio { get; set; }
        public string backgroundUrl { get; set; }
        public string end { get; set; }
        public Exhibit[] exhibits { get; set; }
        public string id { get; set; }
        public string offsetY { get; set; }
        public string start { get; set; }
        public Timeline[] timelines { get; set; }
        public string title { get; set; }
    }

    public class Exhibit
    {
        public string __type { get; set; }
        public bool IsCirca { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedTime { get; set; }
        public List<ContentItem> contentItems { get; set; }
        public string id { get; set; }
        public string offsetY { get; set; }
        public Int64 time { get; set; }
        public string title { get; set; }
        public string ParentTimeLineId { get; set; }
    }

    public class ContentItem
    {
        public string __type { get; set; }
        public Int64 year { get; set; }
        public string attribution { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string mediaSource { get; set; }
        public string mediaType { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public string uri { get; set; }
        public string ParentExhibitId { get; set; }
        public Int64 ParentExhibitTime { get; set; }
    }
}
