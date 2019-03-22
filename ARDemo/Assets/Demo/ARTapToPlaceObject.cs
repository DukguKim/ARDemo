using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlaceObject : MonoBehaviour {

    public GameObject objectToPlace;  // 배치할 오브젝트
    public GameObject placementIndicator; // 배치 위치를 표시

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;


	void Start () {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
	}
	
	void Update () {
        UpdatePlacementPose();
        UpdatePlaceIndicator();

        if (placementPoseIsValid && 
            Input.touchCount> 0 && // 손가락이 스크린에 있는가
            Input.GetTouch(0).phase == TouchPhase.Began) // 손가락의 상태 (막 누르기 시작했는가)
        {
            PlaceObject();
        }
	} 

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation); //오브젝트 생성
    }

    private void UpdatePlaceIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }

    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3( 0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraFoward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraFoward.x, 0, cameraFoward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}


 