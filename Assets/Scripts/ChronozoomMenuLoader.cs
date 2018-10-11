using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//This script is attached to ChronozoomMenuGroup in ChronozoomMenuView scene. It is responsible for loading the data from chronoplay API in order to retrieve list of collections.
public class ChronozoomMenuLoader : MonoBehaviour {

    private List<PlayableCollection> playableCollectionList;
    private const string ChronozoomCollectionsURI = "http://chronoplayapi.azurewebsites.net:80/api/Counts?APIKey=";
    private const string Key = "2c917dc4aaa343a0817688db82ef275d";
    private ChronozoomMenuManager chronozoomMenuManager;
    

    void Start () {
        //Change rotation to face camera
        transform.rotation = Camera.main.transform.rotation;
        playableCollectionList = new List<PlayableCollection>();
        chronozoomMenuManager = GetComponent<ChronozoomMenuManager>();
        StartCoroutine(GetChronozoomCollections());
    }
	
	IEnumerator GetChronozoomCollections()
    {
        Debug.Log("Getting Chronozoom Collections");
        UnityWebRequest www = UnityWebRequest.Get(ChronozoomCollectionsURI + Key);
        // www.timeout = 300;
        yield return www.SendWebRequest();

        // Knowon Error https://issuetracker.unity3d.com/issues/wsa-isnetworkerror-always-return-true-when-running-an-uwp-app-on-86x-architecture
        //if (www.isNetworkError)
        //{
        //    Debug.Log(www.error);
        DeserializeData(www.downloadHandler.text);
        //}
        //else
        //{
        //    Debug.Log("Retrieved Chronozoom Collections Data");
        //    DeserializeData(www.downloadHandler.text);
        //}
    }

    void DeserializeData(string data)
    {
        playableCollectionList = JsonConvert.DeserializeObject<List<PlayableCollection>>(data);
        DisplayData();
    }

    void DisplayData()
    {
        chronozoomMenuManager.Initiate(playableCollectionList);

    }
}

public class PlayableCollection
{
    public int idx { get; set; }
    public string SuperCollection { get; set; }
    public string Collection { get; set; }
    public double? Timeline_Count { get; set; }
    public double? Exhibit_Count { get; set; }
    public bool Publish { get; set; }
    public string CZClone { get; set; }
    public string Language { get; set; }
    public string Comment { get; set; }
    public double? Content_Item_Count { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Title { get; set; }
    public string ImageURL { get; set; }
}
