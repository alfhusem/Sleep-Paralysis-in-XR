using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    public Transform target; // The player's transform
    public float speed = 5f;
    private bool shouldMove = true;

    void Update()
    {
        if (target != null && shouldMove)
        {
            // Check the distance between the quad and the target
            if (Vector3.Distance(transform.position, target.position) < 1f)
            {
                // If the distance is less than 1, stop moving
                shouldMove = false;
            }
            else
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            }
        }
    }
}
