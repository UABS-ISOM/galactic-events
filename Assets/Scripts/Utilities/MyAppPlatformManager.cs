﻿// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System;
using UnityEngine;

namespace GalaxyExplorer
{
    public class MyAppPlatformManager : SingleInstance<MyAppPlatformManager>
    {
        public enum PlatformId
        {
            HoloLens,
            ImmersiveHMD,
            Desktop,
            Phone
        };

        public static PlatformId Platform { get; private set; }

        public static readonly bool SpeechEnabled = false;

        public static float SlateScaleFactor
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                        return 3.0f;
                    case PlatformId.HoloLens:
                    case PlatformId.Desktop:
                        return 1.0f;
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static float MagicWindowScaleFactor
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                        return 3.0f;
                    case PlatformId.HoloLens:
                    case PlatformId.Desktop:
                        return 1.0f;
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static float OrbitalTrailFixedWidth
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                        return 0.0035f;
                    case PlatformId.HoloLens:
                    case PlatformId.Desktop:
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static float GalaxyScaleFactor
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                        return 3.0f;
                    case PlatformId.HoloLens:
                        return 1.0f;
                    case PlatformId.Desktop:
                        return 0.75f;
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static float SolarSystemScaleFactor
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                    case PlatformId.HoloLens:
                        return 1.0f;
                    case PlatformId.Desktop:
                        return 0.35f;
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static float PoiMoveFactor
        {
            get
            {
                float moveFactor = 1f;
                if (ViewLoader.Instance.CurrentView.Equals("SolarSystemView"))
                {
                    moveFactor *= SolarSystemScaleFactor;
                }
                else if (ViewLoader.Instance.CurrentView.Equals("GalaxyView"))
                {
                    moveFactor *= GalaxyScaleFactor;
                }
                return moveFactor;
            }
        }

        public static float PoiScaleFactor
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                        return 3.0f;
                    case PlatformId.HoloLens:
                        return 1.0f;
                    case PlatformId.Desktop:
                        return 0.75f;
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static float SpiralGalaxyTintMultConstant
        {
            get
            {
                switch (Platform)
                {
                    case PlatformId.ImmersiveHMD:
                        return 0.22f;
                    case PlatformId.HoloLens:
                    case PlatformId.Desktop:
                        return 0.3f;
                    default:
                        throw new System.Exception();
                }
            }
        }

        public static event Action MyAppPlatformManagerInitialized;

        public static string DeviceFamilyString = "Windows.Desktop";
        // Use this for initialization
        void Awake()
        {
            switch (DeviceFamilyString)
            {
                case "Windows.Holographic":
                    Platform = PlatformId.HoloLens;
                    break;
                case "Windows.Desktop":
                    if (!UnityEngine.XR.XRDevice.isPresent)
                    {
                        Platform = PlatformId.Desktop;
                    }
                    else
                    {
                        if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
                        {
                            Platform = PlatformId.ImmersiveHMD;
                        }
                        else
                        {
                            Platform = PlatformId.HoloLens;
                        }
                    }
                    break;
                case "Windows.Mobile":
                    Platform = PlatformId.Phone;
                    break;
                default:
                    Platform = PlatformId.Desktop;
                    break;
            }
            Debug.LogFormat("MyAppPlatformManager says its Platform is {0}", Platform.ToString());
            if (MyAppPlatformManagerInitialized != null)
            {
                MyAppPlatformManagerInitialized();
            }
        }
    }
}