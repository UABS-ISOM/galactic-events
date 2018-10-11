using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

//This script is attached to the ChronozoomBox prefab that becomes instantiated programmatically. It is responsible for moving the box infront of the player when the animation is played.
public class ChronozoomPresentToPlayer : MonoBehaviour {

    //Variable to check if the box has finished moving to the right position
    public bool InPosition
    {
        get
        {
            return inPosition;
        }
    }

    //Variable to check if the current box is active
    public bool Presenting
    {
        get
        {
            return presenting;
        }
    }

    public static ChronozoomPresentToPlayer ActiveExhibit;
    public float PresentationDistance = 1f;
    public float TravelTime = 1f;
    public bool OrientToCamera = true;
    public bool OrientYAxisOnly = true;
    public Transform TargetTransform;

    Vector3 initialPosition;
    Quaternion initialRotation;
    bool presenting = false;
    bool returning = false;
    bool inPosition = false;

    public void Present()
    {
        if (presenting)
            return;

        presenting = true;
        StartCoroutine(PresentOverTime());
    }
    public void Return()
    {
        if (!presenting)
            return;

        returning = true;
    }

    IEnumerator PresentOverTime()
    {

        if (TargetTransform == null)
            TargetTransform = transform;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        Vector3 targetPosition = cameraPosition + cameraForward * PresentationDistance;
        inPosition = false;

        float normalizedProgress = 0f;
        float startTime = Time.time;

        while (!inPosition)
        {
            // Move the object directly in front of player
            normalizedProgress = (Time.time - startTime) / TravelTime;
            TargetTransform.position = Vector3.Lerp(initialPosition, targetPosition, normalizedProgress);
            if (OrientToCamera)
            {
                TargetTransform.rotation = Quaternion.Lerp(TargetTransform.rotation, targetRotation, Time.deltaTime * 10f);
            }
            inPosition = Vector3.Distance(TargetTransform.position, targetPosition) < 0.05f;
            yield return null;
        }

        while (!returning)
        {
            // Wait to be told to return
            yield return null;
        }

        // Move back to our initial position
        inPosition = false;
        normalizedProgress = 0f;
        startTime = Time.time;
        while (normalizedProgress < 1f)
        {
            normalizedProgress = (Time.time - startTime) / TravelTime;
            TargetTransform.position = Vector3.Lerp(targetPosition, initialPosition, normalizedProgress);
            if (OrientToCamera)
            {
                TargetTransform.rotation = Quaternion.Lerp(TargetTransform.rotation, initialRotation, Time.deltaTime * 10f);
            }
            inPosition = Vector3.Distance(TargetTransform.position, initialPosition) < 0.05f;
            yield return null;
        }

        TargetTransform.position = initialPosition;
        TargetTransform.rotation = initialRotation;
        presenting = false;
        returning = false;

        yield break;
    }
}
