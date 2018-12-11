using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class PlotPatternGalaxy
    {
        public float x; // last co-ords
        public float y;
        public float z;

        public float i; // incremented item number / iteration
        public float gap; // size of gap
        public float distance; // distance from centre of spiral
        public float distance_var; // variation per incrementation of the distance (mult)

        public PlotPatternGalaxy()
        {
            // set values to a sensible default when there's not ctor vals supplied
            x = y = i = distance = 0;
            gap = .04f;
            distance_var = .05f;
        }

        public PlotPatternGalaxy(float x, float y, float z, float gap, float distance, float distance_var)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.gap = gap;
            this.distance = distance;
            this.distance_var = distance_var;
        }

        public PlotPoint GetPoint()
        {
            x = Mathf.Sin(i * gap) * distance;
            z = Mathf.Cos(i * gap) * distance;
            y = 0;

            distance = i * distance_var;

            i++;

            return new PlotPoint() { x = x, y = y, z = z };
        }
    }

    public class PlotPoint
    {
        public float x;
        public float y;
        public float z;
    }
}
