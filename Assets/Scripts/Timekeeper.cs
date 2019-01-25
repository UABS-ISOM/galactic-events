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
                data[mode]["year"] += Time.deltaTime / data[mode]["rate"];
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
