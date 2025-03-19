using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpawner : MonoBehaviour
{
    public GameObject prefab;
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
            OVRSemanticClassification classification = hit.collider.gameObject.GetComponent<OVRSemanticClassification>();

            // Check if the hit object has a semantic classification with the label "FLOOR"
            if (classification != null && classification.Contains("FLOOR"))
            {
                // Use the center of the anchor if the label is "FLOOR"
                Vector3 anchorCenter = classification.transform.position;
                Quaternion anchorRotation = Quaternion.LookRotation(playerTransform.position - anchorCenter);
                anchorRotation.x = anchorRotation.z = 0; // Keep the prefab upright, aligned with the floor

                // Update the preview prefab's position to the center of the anchor
                currentPreview.transform.position = anchorCenter;
                currentPreview.transform.rotation = anchorRotation;

                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    // Instantiate the actual prefab at the center of the floor anchor
                    Instantiate(prefab, anchorCenter, anchorRotation);
                }
            }
            else
            {
                // If it's not a "FLOOR" label or no classification, keep the original behavior
                Vector3 directionToPlayer = playerTransform.position - hit.point;
                directionToPlayer.y = 0; // Keep the direction on a horizontal plane
                directionToPlayer.Normalize();

                if (directionToPlayer == Vector3.zero)
                {
                    directionToPlayer = hit.transform.forward;
                }

                Quaternion lookAtPlayerRotation = Quaternion.LookRotation(directionToPlayer, hit.normal);
                currentPreview.transform.position = hit.point;
                currentPreview.transform.rotation = lookAtPlayerRotation;
            }
        }
    }
}
