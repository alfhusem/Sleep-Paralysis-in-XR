using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DevGun : MonoBehaviour
{
    // private GameObject previewPrefab; 
    private GameObject currentPreview; // The current instance of the preview object
    private Transform playerTransform;

    public int step { get; private set; } = -1;
    private Quaternion rotation;

    public GamePhaseManager manager;
    public GameObject shadowPrefab, soundPrefab, doorPrefab, bigDoorPrefab, impPrefab, bansheePrefab, crawlerPrefab;
    public event Action<GameObject> OnObjectPlaced;
    private List<GameObject> previews = new List<GameObject>();
    private GameObject placedObject;

    Vector3 hitpos;
    Vector3 cursorpos;
    float fuck = 0;

    private bool canPressButton = true; 
    private int corner = 0;
    
    
    void Start()
    {
        currentPreview = Instantiate(shadowPrefab);
        playerTransform = manager.playerTransform;
    }
    void Update()
    {   
        Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);
        Vector3 curs = ray.origin + ray.direction.normalized * 1.0f;
        cursorpos = new Vector3(curs.x, curs.y + 1, curs.z);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        { 
            hitpos = hit.point;
            if (step == 5) {
                hitpos = new Vector3(hitpos.x, 2f, hitpos.z);
            }
            if (currentPreview != null) {
                if (step == 3) // Imps
                {
                    currentPreview.transform.position = cursorpos;
                }
                else
                {
                    currentPreview.transform.position = hitpos;
                }   
                if(step == 1 || step == 0){ // DOOR
                    SetDoorRotation(currentPreview.transform.position);
                }
                else 
                {
                    Vector3 directionToPlayer = playerTransform.position - hit.point;
                    directionToPlayer.y = 0;  // This flattens the direction to the horizontal plane
                    Quaternion facePlayer = Quaternion.LookRotation(directionToPlayer, Vector3.up);

                    if (step == 5) {
                        currentPreview.transform.rotation = facePlayer * Quaternion.Euler(40, 0, 0);
                    }
                    else{
                        currentPreview.transform.rotation = facePlayer;
                    }
                }
            }
        }
        bool OPress = Input.GetKeyDown(KeyCode.O);
        if ((OVRInput.GetDown(OVRInput.Button.One) || OPress) && canPressButton)
        {
            if (OPress)
            {
                hitpos = new Vector3(2, 0, 2);
                cursorpos = new Vector3(fuck, fuck, fuck);
                fuck += 0.5f;
            }

            canPressButton = false; // Disable button press
            HandleButtonOnePress();
            StartCoroutine(ResetButtonPress());
        }
   
    }

    void HandleButtonOnePress()
    {
        switch (step)
        {
            case -1: 
                previews.Add(currentPreview);
                PlaceObject(manager.placedObject, hitpos);
                currentPreview = Instantiate(doorPrefab, hitpos, rotation);
                previews.Add(currentPreview);
                manager.wallRotator.EnableWallMesh(false);
                break;
            case 0:
                PlaceObject(manager.placedDoor, hitpos); //Door
                currentPreview = Instantiate(bigDoorPrefab, hitpos, rotation);
                previews.Add(currentPreview); 

                AudioSource knock = manager.placedDoor.transform.GetChild(0).GetComponent<AudioSource>();
                knock.Play();
                StartCoroutine(manager.StopAudioAfterTime(knock, 2f));      
                break;
            case 1:
                PlaceObject(manager.placedBigDoor, hitpos); //Big Door
                currentPreview = Instantiate(soundPrefab, hitpos, rotation);
                previews.Add(currentPreview); 
                break;
            case 2:
                PlaceObject(manager.behindYouAudioSource, new Vector3(hitpos.x, 1.5f, hitpos.z));
                currentPreview = Instantiate(impPrefab, cursorpos, rotation);
                previews.Add(currentPreview); 
                break;
            case 3:
                // Process imps
                foreach (var imp in manager.impsList)
                {
                    PlaceObject(imp, cursorpos);
                }   
                currentPreview = Instantiate(bansheePrefab, hitpos, rotation);
                previews.Add(currentPreview); 
                break;  
            case 4:
                PlaceObject(manager.placedBanshee, hitpos);
                currentPreview = Instantiate(crawlerPrefab, hitpos, rotation);
                previews.Add(currentPreview);
                
                foreach (var imp in manager.imps)
                {
                    imp.Key.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
                }

                break;
            case 5:
                PlaceObject(manager.placedCrawler, hitpos);
                currentPreview = Instantiate(soundPrefab, hitpos, rotation); 
                currentPreview.GetComponent<MeshRenderer>().material.color = Color.white;
                previews.Add(currentPreview);               
                break;
            case 6:
                manager.lightSource.transform.position = new Vector3(hitpos.x, 1.5f, hitpos.z);
                currentPreview.GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case 7:
                manager.corners[corner] = hitpos;
                if (corner == 3) // 4th corner placed
                {
                    manager.wallRotator.EnableWallMesh(true);
                    manager.EnterNextPhase();
                    break;
                }
                currentPreview.GetComponent<MeshRenderer>().material.color = (corner % 2 != 0) ? Color.blue : Color.green;                
                corner++;
                step--;
                break;

        }
        step++;

    }
    IEnumerator ResetButtonPress()
    {
        yield return new WaitForSeconds(0.5f);
        canPressButton = true;
    }
    void PlaceObject(GameObject prefabToPlace, Vector3 pos)
    {   
        //rotation = currentPreview.activeSelf ? currentPreview.transform.rotation : Quaternion.identity;
        rotation = currentPreview.transform.rotation;
        if(step == 2)
        {
            prefabToPlace.transform.position = new Vector3(pos.x, 1.5f, pos.z);
            placedObject = prefabToPlace;
        }
        else
        {
            placedObject = Instantiate(prefabToPlace, pos, rotation);
        }
        OnObjectPlaced?.Invoke(placedObject);
    }

    void SetDoorRotation(Vector3 doorPosition)
    {
        GameObject nearestWall = null;
        float minDistance = float.MaxValue;

        foreach (GameObject wall in manager.wallRotator.rotatedWalls)
        {
            float distance = Vector3.Distance(doorPosition, wall.transform.position);
            if (distance < minDistance)
            {
                nearestWall = wall;
                minDistance = distance;
            }
        }

        if (nearestWall != null)
        {
            currentPreview.transform.rotation = nearestWall.transform.rotation;
        }
    }

    void OnDisable()
    {
        foreach (GameObject preview in previews)
        {
            Destroy(preview);
        }
    }

    void OnEnable()
    {
        //currentPreview = Instantiate(shadowPrefab);
        manager.print("DevGun enabled");
    }

    

}

