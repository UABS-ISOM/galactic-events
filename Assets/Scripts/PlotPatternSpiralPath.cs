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

            Quaternion rotation = spiralContainer.transform.localRotation;
            rotation.y = -90;

            spiralContainer.transform.localRotation = rotation;

            for (int i = 0; i < itemCount; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(spiralContainer.transform);
                sphere.transform.localScale = new Vector3(.1f, .1f, .1f);
                sphere.transform.localPosition = spiralPattern.GetPoint();
            }
        }

        public Vector3 GetPoint()
        {
            return spiralPattern.GetPoint();
        }
    }
}
