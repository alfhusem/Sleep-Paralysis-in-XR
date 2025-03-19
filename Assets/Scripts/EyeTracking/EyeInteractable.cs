using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    [SerializeField] private UnityEvent<GameObject> OnObjectHover; 
    
    private MeshRenderer meshRenderer; 
    private float hoverTime = 0f; // Time the object has been hovered over
    private float hoverDuration = 1f; // Duration to look at the object before it is destroyed
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    void Update() 
    {
        if (IsHovered)
        {
            // // Increment the hoverTime by the time elapsed since last frame
            // hoverTime += Time.deltaTime;

            // // Check if the hoverTime exceeds the threshold
            // if (hoverTime >= hoverDuration)
            // {
            //     // If so, destroy the object
            //     Destroy(gameObject);
            // }

            meshRenderer.enabled = false;
            // OnObjectHover?.Invoke(gameObject);
        }
        else
        {
            // Reset hover time since the object is no longer being hovered over
            hoverTime = 0f;
            meshRenderer.enabled = true;
        }
    }
}
