using UnityEngine;

namespace GalaxyExplorer {

    public class GalaticController : MonoBehaviour
    {

        public float speedMultiplier = 1f;
        public static GalaticController instance = null;

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

    }

}