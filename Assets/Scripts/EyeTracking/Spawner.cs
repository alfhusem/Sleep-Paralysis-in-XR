using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject quadPrefab; // Assign your quad prefab in the inspector
    public float spawnRate = 2f; // How often quads are spawned, in seconds
    public float spawnDistanceThreshold = 30f; // The angle within which the spawner will not spawn quads

    private float spawnCooldown;
    public Transform playerHead;
    public Transform target;

    void Update()
    {
        Vector3 toSpawner = transform.position - playerHead.position;
        float angleToGaze = Vector3.Angle(playerHead.forward, toSpawner);

        // If the player is facing away from the spawner (e.g., angle greater than 90 degrees)
        if  (Time.time >= spawnCooldown) //(angleToGaze > 90f &&
        {
            SpawnQuad();
            spawnCooldown = Time.time + spawnRate;
        }
    }


    void SpawnQuad()
    {
        // Calculate the direction from the spawner to the target
        Vector3 directionToTarget = target.position - transform.position;
        // Create a rotation that looks along the directionToTarget
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        // Instantiate the quad with the calculated rotation
        GameObject quad = Instantiate(quadPrefab, transform.position, lookRotation);
        MoveTowardsTarget moveTowards = quad.GetComponent<MoveTowardsTarget>();
        if (moveTowards != null)
        {
            moveTowards.target = target; 
        }
        // Ensure the Rigidbody and MeshCollider are set up correctly
        Rigidbody rb = quad.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.WakeUp(); // Just in case the Rigidbody is asleep
        }
        
        MeshCollider collider = quad.GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }


}