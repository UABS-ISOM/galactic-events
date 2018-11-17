using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace GalaxyExplorer
{
    public class BackendService : MonoBehaviour
    {
        private string baseURL = "http://localhost:3000";
        public static BackendService instance = null;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }


            else if (instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void LogAction(string actionTarget)
        {
            WWWForm formData = new WWWForm();
            formData.AddField("target", actionTarget);

            UnityWebRequest.Post(baseURL + "/api/action-log", formData).SendWebRequest();
        }

    }
}