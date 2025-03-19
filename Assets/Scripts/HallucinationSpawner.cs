using UnityEngine;

public class HallucinationSpawner : MonoBehaviour
{
    public GameObject prefab;
    //public Transform playerTransform;


    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            // Vector3 directionToPlayer = playerTransform.position - transform.position;

            // Vector3 projectedDirectionToPlayer = Vector3.ProjectOnPlane(directionToPlayer, transform.up).normalized;

            // // If there is no direction after the projection, default to global forward direction
            // if (projectedDirectionToPlayer == Vector3.zero)
            // {
            //     projectedDirectionToPlayer = Vector3.forward;
            //     Debug.Log("Default direction to Vector3.forward");
            // }

            // // Calculate the rotation needed to look at the player while being normal to the wall
            // Quaternion lookAtPlayerRotation = Quaternion.LookRotation(projectedDirectionToPlayer, transform.up);

            // // Update the preview prefab's position and rotation
            // currentPreview.transform.position = hit.point;
            // currentPreview.transform.rotation = lookAtPlayerRotation;
            // // Instantiate the actual prefab facing the player
            // Instantiate(prefab, hit.point, lookAtPlayerRotation);
            Instantiate(prefab, transform.position, transform.rotation);
        }
       
    }
}
