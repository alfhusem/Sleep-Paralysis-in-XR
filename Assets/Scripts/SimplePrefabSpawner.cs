using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    public GameObject prefab_idle;
    public GameObject prefab_crawl;
    public GameObject prefab_spawner;
    public GameObject previewPrefab;
    private GameObject currentPreview;
    public Transform playerTransform;

    private void Start()
    {
        currentPreview = Instantiate(previewPrefab);
    }
 
    private void Update()
    {
        Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Calculate the direction from the hit point to the player's head
            Vector3 directionToPlayer = playerTransform.position - hit.point;
            // Project the direction onto the plane defined by the hit.normal
            Vector3 projectedDirectionToPlayer = Vector3.ProjectOnPlane(directionToPlayer, hit.normal).normalized;

            // If there is no direction after the projection, default to global forward direction
            if (projectedDirectionToPlayer == Vector3.zero)
            {
                projectedDirectionToPlayer = Vector3.forward;
            }

            // Calculate the rotation needed to look at the player while being normal to the wall
            Quaternion lookAtPlayerRotation = Quaternion.LookRotation(projectedDirectionToPlayer, hit.normal);

            // Update the preview prefab's position and rotation
            currentPreview.transform.position = hit.point;
            currentPreview.transform.rotation = lookAtPlayerRotation; 

            // if (OVRInput.GetDown(OVRInput.Button.One))
            // {
            //     // Instantiate the actual prefab facing the player
            //     Instantiate(prefab_idle, hit.point, lookAtPlayerRotation);
            // }
            // if (OVRInput.GetDown(OVRInput.Button.Two))
            // {
            //     // Instantiate the actual prefab facing the player
            //     Instantiate(prefab_crawl, hit.point, lookAtPlayerRotation);
            // }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                // Instantiate the actual prefab facing the player
                Instantiate(prefab_spawner, hit.point, lookAtPlayerRotation);
            }
        }
    }
}
 