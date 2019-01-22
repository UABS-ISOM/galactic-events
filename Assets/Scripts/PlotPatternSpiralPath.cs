﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class PlotPatternSpiralPath : MonoBehaviour
    {
        private PlotPatternGalaxy spiralPattern = new PlotPatternGalaxy();
        private GameObject spiralContainer;
        private GameObject bezPath;
        private int bezPathMax = 12; // number of points allowed on the path at once

        private GameObject spiralLastNode;

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

                if (i == itemCount) spiralLastNode = sphere;
            }

            bezPath = new GameObject("BezierPath");
            bezPath.transform.SetParent(spiralContainer.transform);
            bezPath.transform.localPosition = new Vector3(0, 0, 0);

            BezierCurve curve = bezPath.AddComponent<BezierCurve>();
            curve.points = new Vector3[]
            {
                new Vector3(.4f, -.165f, .25f),
                new Vector3(.4f, -.7f, -.65f),
                new Vector3(1.2f, -1.23f, -1.5f)
            };

            for (int i = 0; i < bezPathMax; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(bezPath.transform);
                sphere.transform.localScale = new Vector3(.01f, .01f, .01f);
                sphere.transform.position = curve.GetPoint(i / (float)bezPathMax);
            }
        }

        public Vector3 GetPoint()
        {
            return spiralPattern.GetPoint();
        }
    }
}
