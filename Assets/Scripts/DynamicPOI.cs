using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class DynamicPOI : MonoBehaviour
    {
        public string context; // which POIs to get for this scene

        private GameObject parent_POIRot;

        void Start()
        {
            parent_POIRot = GameObject.Find("POIRotation");

            // get the elements to plot
            PlotPOI("test", null, new Vector3(-1.5f, 0, -.3f), new GameObject());
        }

        // plot a point of interest programmatically
        public void PlotPOI(string name, string transitionScene, Vector3 position, GameObject card)
        {
            // setup structure
            GameObject parent_gameObj = new GameObject(name);
            parent_gameObj.transform.position = position;
            parent_gameObj.transform.SetParent(parent_POIRot.transform);

            GameObject poi = new GameObject("POI");
            poi.transform.SetParent(parent_gameObj.transform);
            GameObject poi_faceCamera = new GameObject("FaceCamera");
            poi_faceCamera.transform.SetParent(poi.transform);
            GameObject poi_target = new GameObject("Target");
            poi_target.transform.SetParent(poi_faceCamera.transform);
            GameObject target_ui = new GameObject("UI_POI");
            target_ui.transform.SetParent(poi_target.transform);


            GameObject poiLineBase = new GameObject("POILineBase");
            poiLineBase.transform.SetParent(parent_gameObj.transform);
            GameObject poiLineBase_lb = new GameObject("LineBase");
            poiLineBase_lb.transform.SetParent(poiLineBase.transform);

            card.transform.SetParent(poi_faceCamera.transform); // add the supplied card here

            SphereCollider parent_ColliderCmp = parent_gameObj.AddComponent<SphereCollider>();
            parent_ColliderCmp.radius = .015f;

            //
            // parent_gameObj components
            PoiResizer parent_PoiResizerCmp = parent_gameObj.AddComponent<PoiResizer>();
            parent_PoiResizerCmp.PoiCard = card;
            parent_PoiResizerCmp.PoiIndicator = target_ui;
            parent_PoiResizerCmp.movePoiStartingPosition = true;

            //
            // poi components
            AudioSource poi_audioSourceCmp = poi.AddComponent<AudioSource>();
            poi_audioSourceCmp.playOnAwake = true;
            poi_audioSourceCmp.priority = 128;
            poi_audioSourceCmp.volume = 1;
            poi_audioSourceCmp.pitch = 1;
            poi_audioSourceCmp.reverbZoneMix = 1;

            ScaleWithDistance poi_scaleWithDistanceCmp = poi.AddComponent<ScaleWithDistance>();
            poi_scaleWithDistanceCmp.IntendedViewDistance = 1;
            poi_scaleWithDistanceCmp.MinScale = .01f;
            poi_scaleWithDistanceCmp.MaxScale = 100;

            FaceCamera poi_faceCameraCmp = poi.AddComponent<FaceCamera>();
            poi_faceCameraCmp.rotationOffset = new Vector3(0, 0, 0);
            poi_faceCameraCmp.forceToWorldUp = true;

            TargetSizer poi_targetSizerCmp = poi.AddComponent<TargetSizer>();

            BillboardLine poi_billboardLineCmp = poi.AddComponent<BillboardLine>();
            poi_billboardLineCmp.material = (Material)Resources.Load("/Assets/Materials/Utilities/PointOfInterestLine", typeof(Material));
            poi_billboardLineCmp.width = .001f;
            poi_billboardLineCmp.bottomOffset = new Vector3(0, .0018f, 0);

            PointOfInterest poi_pointOfInterestCmp = poi.AddComponent<PointOfInterest>();
            poi_pointOfInterestCmp.Indicator = poi_target;
            poi_pointOfInterestCmp.TargetPoint = parent_ColliderCmp;
            poi_pointOfInterestCmp.IndicatorLine = poi_billboardLineCmp;
            poi_pointOfInterestCmp.IndicatorOffset = new Vector3(0, .11f, 0);
            poi_pointOfInterestCmp.IndicatorDefaultColor = new Color(.4235294f, .8117647f, .8666667f, .5490196f);
            poi_pointOfInterestCmp.IndicatorHighlightWidth = .0015f;
            poi_pointOfInterestCmp.Description = card;
            poi_pointOfInterestCmp.TransitionScene = transitionScene;
            poi_pointOfInterestCmp.HighlightSound = "i_ui_rollover"; // appears to be the default
            poi_pointOfInterestCmp.AirtapSound = (AudioClip)Resources.Load("/Assets/Audio/UI/ui_galaxy_airtap_01", typeof(AudioClip));

            MaterialsFader poi_materialsFaderCmp = poi.AddComponent<MaterialsFader>();
            poi_materialsFaderCmp.materials = new Material[3]
            {
                (Material)Resources.Load("/Assets/UI/Tesseract/Materials/line_end", typeof(Material)),
                (Material)Resources.Load("/Assets/UI/Tesseract/Materials/UI_Info", typeof(Material)),
                (Material)Resources.Load("/Assets/Materials/Utilities/PointOfInterestLine", typeof(Material))
            };

            //
            // poi_faceCamera components
            FaceCamera poi_faceCamera_faceCameraCmp = poi_faceCamera.AddComponent<FaceCamera>();
            poi_faceCamera_faceCameraCmp.rotationOffset = new Vector3(0, 180, 0);

            ScaleWithDistance poi_faceCamera_scaleWithDistanceCmp = poi_faceCamera.AddComponent<ScaleWithDistance>();
            poi_faceCamera_scaleWithDistanceCmp.IntendedViewDistance = 2.8f;
            poi_faceCamera_scaleWithDistanceCmp.MaxScale = 5;
            poi_faceCamera_scaleWithDistanceCmp.RescaleEveryFrame = true;

            //
            // target_ui components
            target_ui.transform.position = new Vector3(0, .0175f, 0);
            target_ui.transform.localScale = new Vector3(.03f, .03f, .03f);

            BoxCollider target_ui_boxColliderCmp = target_ui.AddComponent<BoxCollider>();
            target_ui_boxColliderCmp.center = new Vector3(9.536743e-07f, -2.980232e-07f, -1.907349e-06f); // maybe a problem
            target_ui_boxColliderCmp.size = new Vector3(1, 1, .01f);

            MeshRenderer target_ui_meshRendererCmp = target_ui.AddComponent<MeshRenderer>();
            target_ui_meshRendererCmp.materials = new Material[1]
            {
                (Material)Resources.Load("/Assets/UI/Tesseract/Materials/UI_POI", typeof(Material))
            };

            MeshFilter target_ui_meshFilterCmp = target_ui.AddComponent<MeshFilter>();
            target_ui_meshFilterCmp.mesh = (Mesh)Resources.Load("/Assets/Quad", typeof(Mesh)); // might not work

            //
            // poiLineBase components
            poiLineBase.transform.localScale = new Vector3(.05f, .05f, .05f);

            ScaleWithDistance poiLineBase_scaleWithDistanceCmp = poiLineBase.AddComponent<ScaleWithDistance>();
            poiLineBase_scaleWithDistanceCmp.IntendedViewDistance = 1;
            poiLineBase_scaleWithDistanceCmp.MinScale = .01f;
            poiLineBase_scaleWithDistanceCmp.MaxScale = 100;

            //
            // poiLineBase_lb components
            poiLineBase_lb.transform.localScale = new Vector3(.1f, .1f, .1f);

            DynamicMeshCombiner poiLineBase_lb_dynMeshCombCmp = poiLineBase_lb.AddComponent<DynamicMeshCombiner>();
            poiLineBase_lb_dynMeshCombCmp.type = "LineBase";

            FaceCamera poiLineBase_lb_faceCameraCmp = poiLineBase_lb.AddComponent<FaceCamera>();
            poiLineBase_lb_faceCameraCmp.rotationOffset = new Vector3(0, 0, 0);

            MeshRenderer poiLineBase_lb_meshRendererCmp = poiLineBase_lb.AddComponent<MeshRenderer>();
            poiLineBase_lb_meshRendererCmp.materials = new Material[1]
            {
                (Material)Resources.Load("/Assets/UI/Tesseract/Materials/line_end", typeof(Material))
            };

            MeshCollider poiLineBase_lb_meshColliderCmp = poiLineBase_lb.AddComponent<MeshCollider>();
            poiLineBase_lb_meshColliderCmp.cookingOptions =
                MeshColliderCookingOptions.CookForFasterSimulation &
                MeshColliderCookingOptions.EnableMeshCleaning &
                MeshColliderCookingOptions.WeldColocatedVertices;
            poiLineBase_lb_meshColliderCmp.sharedMesh = (Mesh)Resources.Load("/Assets/Quad", typeof(Mesh)); // might not work

            MeshFilter poiLineBase_lb_meshFilterCmp = poiLineBase_lb.AddComponent<MeshFilter>();
            poiLineBase_lb_meshFilterCmp.mesh = (Mesh)Resources.Load("/Assets/Quad", typeof(Mesh)); // might not work
        }
    }
}
