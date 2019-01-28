using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public enum TimeMode
    {
        Galaxy,
        Stop
    }

    public class Timekeeper : MonoBehaviour
    {
        public List<Action<float>> updateActions = new List<Action<float>>();
        public Dictionary<TimeMode, Dictionary<string, float>> data =
            new Dictionary<TimeMode, Dictionary<string, float>>()
            {
                { TimeMode.Galaxy, new Dictionary<string, float>() {
                    { "rate", .000000355f }, // one year in game time seconds
                    { "year", 0f }
                } }
            };

        private TimeMode mode = TimeMode.Stop;

        void Update()
        {
            if (mode == TimeMode.Galaxy)
            {
                float year = Time.deltaTime / data[mode]["rate"];
                if (year != data[mode]["year"])
                {
                    data[mode]["year"] += year;
                    foreach (Action<float> action in updateActions)
                        action(data[mode]["year"]); // execute any action callbacks
                }
            }
        }

        public float GetYear()
        {
            return data[mode]["year"];
        }

        public void SwitchMode(TimeMode mode)
        {
            if (this.mode != mode)
            {
                if (this.mode != TimeMode.Stop)
                    data[this.mode]["year"] = 0f; // clear the year counter
                this.mode = mode;
            }
        }
    }
}
