using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class PlotPatternSpiralPath : MonoBehaviour
    {
        private PlotPatternGalaxy spiralPattern = new PlotPatternGalaxy();
        private GameObject spiralContainer;

        public void Setup(int itemCount)
        {
            spiralContainer = new GameObject("SpiralContainer");
            spiralContainer.transform.SetParent(transform);
            spiralContainer.transform.localPosition = new Vector3(4, 1.75f, -.35f);

            spiralContainer.transform.rotation = Quaternion.Euler(new Vector3(-117, 200, 125));

            for (int i = 0; i < itemCount; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(spiralContainer.transform);
                sphere.transform.localScale = new Vector3(.01f, .01f, .01f);
                sphere.transform.localPosition = spiralPattern.GetPoint();
            }
        }

        public Vector3 GetPoint()
        {
            return spiralPattern.GetPoint();
        }
    }
}
