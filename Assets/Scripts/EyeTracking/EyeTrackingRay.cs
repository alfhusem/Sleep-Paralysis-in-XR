using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTrackingRay : MonoBehaviour
{

    [SerializeField]
    private LayerMask layersToInclude; 

    private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();

    // Start is called before the first frame update

    void FixedUpdate() 
    { 
        RaycastHit hit; 
        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward)*10; // !
        if(Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersToInclude)) 
        {
            UnSelect(); 
            var eyeInteractable = hit.transform.GetComponent<EyeInteractable>(); 
            eyeInteractables.Add(eyeInteractable); 
            eyeInteractable.IsHovered = true; 
        } 
        else 
        {

            UnSelect(true);
        }
    }
    
    void UnSelect(bool clear = false)
    {
        foreach (var interactable in eyeInteractables)
        {
            interactable.IsHovered = false;
        }
        if(clear)
        {
            eyeInteractables.Clear();
        }
    }
}