using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallRotator : MonoBehaviour
{

    public GamePhaseManager manager;    
    public GameObject wallParent;
    public GameObject floor;
    public Material blackMaterial;
    public List<GameObject> rotatedWalls = new List<GameObject>();
    public float height;

    private float delayBeforeChecking = 0.5f;
    GameObject grandParent;
    

    void Start()
    {
        grandParent = new GameObject("WallCopies");
        Invoke("RotateWalls", delayBeforeChecking);
    }

    private void RotateWalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (var obj in allObjects)
        {
            // Check if the object is active, matches the naming criterion, and hasn't been tagged yet
            if (obj.activeInHierarchy && obj.name == "WALL_FACE" && obj.tag != "RotatedWall")
            {
                // Rotate the wall 180 degrees around the Y axis
                obj.transform.Rotate(0, 180, 0, Space.Self);
                obj.tag = "RotatedWall"; // Mark as rotated
                rotatedWalls.Add(obj);

                // If we haven't saved a parent yet, and this object has a parent
                if (wallParent == null && obj.transform.parent != null)
                {
                    wallParent = obj.transform.parent.gameObject; // Save the parent GameObject
                    wallParent.transform.SetParent(grandParent.transform, true); // Deal with this later if all else works
                }
            }
        }
        CleanVirtualHome();
    }

    private void CleanVirtualHome() {
        for (int i = 0; i < wallParent.transform.childCount; i++)
        {
            GameObject child = wallParent.transform.GetChild(i).gameObject;
            if (child.activeInHierarchy && child.name == "FLOOR")
            {
                floor = child;
                floor.transform.SetParent(grandParent.transform, true);
                manager.print("Floor found");
            }
            else if (child.activeInHierarchy && child.name != "FLOOR" && child.name != "WALL_FACE")
            {
                Destroy(child);
                Debug.Log("Destroyed " + child.name);
            }
        }
    }

    public void SetWallParentActive(bool isActive)
    {
        // if (wallParent != null)
        // {
        //     wallParent.SetActive(isActive);
        // }
        // if(floor != null)
        // {
        //     floor.SetActive(isActive);
        // }
        if (grandParent != null)
        {
            grandParent.SetActive(isActive);
        }
    }

    public void ExtendRoomDownwards()
    {
        if (wallParent != null)
        {
            // Calculate the downward offset based on the original parent's bounds
            Renderer renderer = wallParent.GetComponentInChildren<Renderer>();
            height = renderer.bounds.size.y;
            Vector3 offset = new Vector3(0, -height, 0);

            // Number of copies to create
            int numberOfCopies = 4; // was 6
            int additionalBlackCopies = 40;
            GameObject[] gameObjects = new GameObject[numberOfCopies + additionalBlackCopies];  // Array to hold references

            // Create multiple copies in a for loop
            for (int i = 0; i < numberOfCopies; i++)
            {
                gameObjects[i] = Instantiate(wallParent, wallParent.transform.position + offset * (i + 1), Quaternion.identity, grandParent.transform);
                gameObjects[i].name = "WallParent_Copy" + i;
            }

            GameObject initialBlackCopy = Instantiate(wallParent, wallParent.transform.position + offset * (numberOfCopies + 1), Quaternion.identity, grandParent.transform);
            initialBlackCopy.name = "WallParent_BlackCopyInitial";
            ChangeMaterialRecursively(initialBlackCopy, blackMaterial);

            for (int i = 0; i < additionalBlackCopies; i++)
            {
                int index = numberOfCopies + i;
                gameObjects[index] = Instantiate(initialBlackCopy, wallParent.transform.position + offset * (index + 1), Quaternion.identity, grandParent.transform);
                gameObjects[index].name = "WallParent_BlackCopy" + i;
            }
 
            if (floor != null)
            {
                Vector3 floorPosition = new Vector3(floor.transform.position.x, wallParent.transform.position.y + offset.y * (numberOfCopies + additionalBlackCopies), floor.transform.position.z);
                // Set floor's y to be below the last copy
                GameObject floorCopy = Instantiate(floor, floorPosition, floor.transform.rotation);
                manager.floorPosY = floorPosition.y;
                ChangeMaterialRecursively(floorCopy, blackMaterial);
                floor.SetActive(false); // Disable the original floor
            }
        }
    }
    private void ChangeMaterialRecursively(GameObject obj, Material newMaterial)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }
    }

    public void EnableWallMesh(bool enable)
    {
        foreach (GameObject wall in rotatedWalls)
        {
            Transform wallMainTransform = wall.transform.Find("VH_WALL(PrefabSpawner Clone)/HOLDER/Wall_main");
            if (wallMainTransform != null)
            {
                MeshRenderer renderer = wallMainTransform.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = enable;
                }
                else
                {
                    Debug.Log("Renderer is null");
                }
            } 
            else
            {
                Debug.Log("Wall main transform is null");
            }
        }      
    }
}