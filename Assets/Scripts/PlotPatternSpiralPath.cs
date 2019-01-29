using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class PlotPatternSpiralPath : MonoBehaviour
    {
        public GameObject spiralContainer;
        public GameObject spiralPath;
        public GameObject bezPath;

        public float pathResolution = 24f;

        private PlotPatternGalaxy spiralPattern = new PlotPatternGalaxy();

        public void Setup(int itemCount)
        {
            spiralContainer = new GameObject("SpiralContainer");
            spiralContainer.transform.SetParent(transform);

            spiralPath = new GameObject("Spiral");
            spiralPath.transform.SetParent(spiralContainer.transform);
            spiralPath.transform.localPosition = new Vector3(-1.9f, 1.7f, 4.2f);
            spiralPath.transform.rotation = Quaternion.Euler(new Vector3(-38, 35, -70));

            for (int i = 0; i < itemCount; i++)
            {
                //GameObject sphere = new GameObject(string.Format("Node {0}", i));
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(spiralPath.transform);
                sphere.transform.localScale = new Vector3(.01f, .01f, .01f);
                sphere.transform.localPosition = spiralPattern.GetPoint();
            }

            bezPath = new GameObject("BezierPath");
            bezPath.transform.SetParent(spiralContainer.transform);
            //bezPath.transform.localPosition = new Vector3(0, 0, 0);

            BezierCurve curve = bezPath.AddComponent<BezierCurve>();
            curve.points = new Vector3[]
            {
                new Vector3(-1.5738f, 1.358f, 4.3781f),
                new Vector3(-1.395f, .98f, 3.35f),
                new Vector3(-.783f, .536f, 2.676f)
            };

            for (int i = 0; i < pathResolution; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(bezPath.transform);
                sphere.transform.localScale = new Vector3(.01f, .01f, .01f);
                sphere.transform.position = curve.GetPoint(i / pathResolution);
            }
        }

        public Transform GetSpiralNode(int index)
        {
            if (index < spiralPath.transform.childCount)
                return spiralPath.transform.GetChild(index);
            else return null;
        }

        public Transform GetPathNode(int index)
        {
            if (index < bezPath.transform.childCount)
                return bezPath.transform.GetChild(index);
            else return null;
        }
    }
}
