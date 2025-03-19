using UnityEngine;
using TMPro;

public class DistanceToWallVisualizer : MonoBehaviour
{
    public TextMeshProUGUI distanceText;

    private OVRSceneManager ovrSceneManager;
    private OVRSceneRoom sceneRoom;
    private OVRScenePlane[] roomWalls;


    private void Awake()
    {
        ovrSceneManager = FindObjectOfType<OVRSceneManager>();
        //ovrSceneManager.RequestSceneCapture();
        ovrSceneManager.SceneModelLoadedSuccessfully += SceneLoaded;
    }

    private void SceneLoaded()
    {
        sceneRoom = FindObjectOfType<OVRSceneRoom>();
        roomWalls = sceneRoom.Walls;
    }

    private void Update()
    {
        if (sceneRoom != null)
        {
            Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            OVRScenePlane nearestWallToController = FindNearestWall(controllerPosition);

            if (nearestWallToController != null)
            {
                float distanceToController = CalculateDistanceToPlane(controllerPosition, nearestWallToController);
                distanceText.text = "Distance from controller to nearest wall: " + distanceToController.ToString("F2");
            }
        }
    }

    private OVRScenePlane FindNearestWall(Vector3 position)
    {
        OVRScenePlane nearestWall = null;
        float nearestDistance = float.MaxValue;

        foreach (var wall in roomWalls)
        {
            float distance = CalculateDistanceToPlane(position, wall);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestWall = wall;
            }
        }

        return nearestWall;
    }

    private float CalculateDistanceToPlane(Vector3 position, OVRScenePlane wall)
    {
        Vector3 wallNormal = wall.transform.forward;
        float wallDistance = -Vector3.Dot(wallNormal, wall.transform.position);
        float distance = Mathf.Abs(Vector3.Dot(wallNormal, position) + wallDistance) / wallNormal.magnitude;

        return distance;
    }
}
