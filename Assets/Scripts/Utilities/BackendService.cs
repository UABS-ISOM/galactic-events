using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace GalaxyExplorer
{
    public class BackendService : MonoBehaviour
    {

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

            UnityWebRequest rq = UnityWebRequest.Post("http://localhost:3000/api/action-log", formData);
            rq.SendWebRequest();
        }

    }
}