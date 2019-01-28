using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GalaxyExplorer
{
    public class Timepiece : MonoBehaviour
    {
        private TextMesh tooltip;
        void Start()
        {
            tooltip = transform.Find("Tooltip").GetComponent<TextMesh>();
            GameObject.Find("/ViewLoader").GetComponent<Timekeeper>()
                .updateActions.Add(year =>
                {
                    tooltip.text = string.Format("{0} years",
                        year.ToString("N0", CultureInfo.InvariantCulture));
                });
        }
    }
}
