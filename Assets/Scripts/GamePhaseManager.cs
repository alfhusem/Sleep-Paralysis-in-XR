using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GamePhaseManager : MonoBehaviour
{
    public DevGun devGun;
    public GameObject placedObject, placedDoor, placedBigDoor, placedBanshee, placedCrawler, lovecraft, bansheeCloseup, placedBansheeCloseup, spider, bigSpider, shadowmanFacePrefab;
    public Dictionary<GameObject, ImpOrbitData> imps = new Dictionary<GameObject, ImpOrbitData>();
    public List<GameObject> impsList = new List<GameObject>();
    public GameObject shadowmanFace;
    public OVRPassthroughLayer passthroughLayer;
    public GameObject lightSource; // Light
    public AudioSource dreamAudioSource, screamAudioSource, bansheeScreanAudioSource, eclipseAudioSource, horrorAudioSource, batsAudioSource, whisperInEarAudioSource, crazyWhispersAudioSource;
    // public AudioSource behindYouAudioSource;
    // public GameObject knockingAudioSource;
    public GameObject behindYouAudioSource;
    public OVRPassthroughLayer dreamLayer;
    public Transform playerTransform;
    public new Camera camera;
    public WallRotator wallRotator;
    public float currentBrightness = 0;
    public TextMeshProUGUI debugText;
    public GameObject video;
    public bool gravityEnabled = false;
    public GameObject fallingCamera;
    public Material dreamSkybox, castleSkybox, blackSkybox, darkForestSkybox;

    private Dictionary<int, GamePhase> phases = new Dictionary<int, GamePhase>();
    private int currentPhaseIndex = -1;
    private bool canPressButton = true; // Control button press
    private float velocity = 0;
    public float floorPosY;
    int directionToggle = 1;
    public bool impsFlyAway = false;
    public Vector3[] corners = new Vector3[4];
    public List<GameObject> spiders = new List<GameObject>();
    public List<GameObject> shadowmen = new List<GameObject>();
    public float spiderFloor = 0;
    public float spiderStart;
    public float spiderSpeed = 0.35f;

    void Start()
    {
        InitializePhases();
        EnterNextPhase();
    }

    private void InitializePhases()
    {
        phases[0] = new SetupPhase(this, devGun);
        phases[1] = new StartPhase(this);
        phases[2] = new KnockingPhase(this);
        phases[3] = new ShadowPhase(this);
        phases[4] = new BatPhase(this);
        phases[5] = new DreamPhase(this);
        phases[6] = new BansheePhase(this);
        phases[7] = new EclipsePhase(this);
        phases[8] = new CorridorPhase(this);
        phases[9] = new CrawlerPhase(this);
        phases[10] = new SpiderPhase(this);
        phases[11] = new DarkForestPhase(this);
        phases[12] = new EndPhase(this);
    }

    void Update()
    {
        // currentPhaseIndex <= 1
        if ((currentPhaseIndex == 1 && OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.P))  && canPressButton)
        {
            canPressButton = false; // Disable button press
            EnterNextPhase();
            StartCoroutine(ResetButtonPress());
            Debug.Log("Button Two pressed");
        }
        if (spiders.Count > 0)
        {
            int count = 0;
            foreach (GameObject spider in spiders)
            {
                if (spider != null)
                {
                    LineRenderer lineRenderer = spider.GetComponent<LineRenderer>();

                    // Check if spider's position y is greater than 0 to allow movement and line rendering
                    if (spider.transform.position.y > spiderFloor + 0.3f)
                    {
                        count++;
                        float speed = spiderSpeed;
                        if (count % 2 == 0) speed -= 0.05f;
                        if (count % 3 == 0) speed += 0.05f;

                        // Move the spider downwards
                        spider.transform.position += Vector3.down * speed * Time.deltaTime;

                        // Update line renderer positions
                        if (lineRenderer != null)
                        {
                            lineRenderer.positionCount = 2;
                            lineRenderer.SetPosition(0, new Vector3(spider.transform.position.x, spiderStart, spider.transform.position.z));
                            lineRenderer.SetPosition(1, spider.transform.position);
                        }
                    }
                    else if (lineRenderer != null)
                    {
                        // Disable the LineRenderer if the spider's y position is 0 or less
                        lineRenderer.enabled = false;
                        spider.transform.rotation = Quaternion.Euler(0, spider.transform.rotation.eulerAngles.y, 0);
                        spider.transform.position = new Vector3(spider.transform.position.x, spiderFloor, spider.transform.position.z);
                    }
                }
            }
        }
        if (shadowmen.Count > 0)
        {
            foreach (GameObject shadowman in shadowmen)
            {
                if (shadowman != null)
                {
                    Animator animator = shadowman.GetComponent<Animator>();
                    Vector3 positionA = shadowman.transform.position;
                    Vector3 positionB = playerTransform.position;
                    positionA.y = 0;
                    positionB.y = 0;
                    float distanceToPlayer = Vector3.Distance(positionA, positionB);
                    
                    Debug.Log("Dis " + distanceToPlayer + " animator " + animator.GetBool("isWalking"));
                    if (animator.GetBool("isWalking") && distanceToPlayer < 25f)
                    {
                        animator.SetBool("isWalking", false);
                    }
                    if (animator.GetBool("isRunning") && distanceToPlayer < 2f)
                    {
                        EnterNextPhase();
                    }
                }
            }
        }

     
    }

    void FixedUpdate()
    {
        if (gravityEnabled) // Per second?
        {
            if (velocity < 25) velocity += 5 * Time.fixedDeltaTime;
            fallingCamera.transform.position += Vector3.down * velocity * Time.fixedDeltaTime;
            fallingCamera.transform.rotation = playerTransform.rotation;

            if(fallingCamera.transform.position.y < floorPosY + velocity)
            {
                gravityEnabled = false;
                fallingCamera.SetActive(false);
                EnterNextPhase(); // From EclipsePhase
            }
        }
        
        foreach (KeyValuePair<GameObject, ImpOrbitData> imp in imps)
        {
            OrbitAroundCursor(imp.Key, imp.Value);
        }
        float speed = 70;
        if (lovecraft.activeSelf)
        {
            Vector3 lovecraftTarget = new Vector3(playerTransform.position.x, playerTransform.position.y - 5f, playerTransform.position.z);
            lovecraft.transform.position = Vector3.MoveTowards(lovecraft.transform.position, lovecraftTarget, speed * Time.deltaTime);
            lovecraft.transform.LookAt(playerTransform);

            if (Vector3.Distance(lovecraft.transform.position, lovecraftTarget) < 8f)
            {
                lovecraft.SetActive(false);
                EnterNextPhase();
            }
        }
    }

    public IEnumerator OpenDoor(GameObject door, float openAngle, float rotationSpeed, float pivot)
    {
        float currentAngle = 0.0f;
        door = door.transform.GetChild(0).gameObject;
        while (currentAngle < openAngle)
        {
            float rotationStep = -rotationSpeed * Time.deltaTime;
            Vector3 hingePoint = door.transform.position + door.transform.right * pivot; // 0.5 units to the left
            door.transform.RotateAround(hingePoint, Vector3.up, rotationStep); // Rotate around the y-axis
            currentAngle -= rotationStep;
            yield return null;
        }
    }

    public void OrbitAroundCursor(GameObject target, ImpOrbitData data)
    {
        if (impsFlyAway)
        {
            Vector3 movement = new Vector3(target.transform.forward.x, 0.5f, target.transform.forward.z).normalized; //Quaternion.Euler(0, 30, 0) * target.transform.forward;
            target.transform.position += movement * data.speed * Time.deltaTime;
        }
        else
        {
            float angle = Time.time * data.speed * data.direction + data.startPos;
            float x = data.centre.x + Mathf.Cos(angle) * data.radius;
            float z = data.centre.z + Mathf.Sin(angle) * data.radius * (1 - data.squeeze);
            float y = data.centre.y + Mathf.Sin(angle) * data.tilt / 90f * data.radius;
            Vector3 newPosition = new Vector3(x, y, z);
            target.transform.position = newPosition;

            Vector3 flyDirection = (newPosition - data.centre).normalized;
            target.transform.rotation = Quaternion.LookRotation(flyDirection, Vector3.up) * Quaternion.Euler(30, -90*data.direction, 0);
        }
    }

    public void IncreaseImpRadius(float factor)
    {
        var keys = new List<GameObject>(imps.Keys);

        foreach (var key in keys)
        {
            ImpOrbitData data = imps[key];
            data.radius *= factor;
            data.speed /= factor;
            imps[key] = data;
        }
    }


    public void DestroyImps()
    {
        foreach (GameObject key in new List<GameObject>(imps.Keys))
        {
            Destroy(key);
        }
        imps.Clear();
    }

    public void SummonImp(GameObject impObject)
    {
        GameObject imp = Instantiate(impObject, playerTransform.position, Quaternion.identity);
        imps[imp] = new ImpOrbitData
        {
            radius = Random.Range(3, 9f),
            speed = Random.Range(1f, 2f),
            startPos = Random.Range(0, 2 * Mathf.PI),
            direction = Random.Range(0, 2) == 0 ? 1 : -1,
            centre = playerTransform.position + 
            new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 5), Random.Range(-0.5f, 0.5f)),
            tilt = Random.Range(-30f, 30f),
            squeeze = Random.Range(0f, 0.5f)
        };
    }

    public void SummonShadowman(Vector3 position) {
        Quaternion facingPlayer = Quaternion.LookRotation(playerTransform.position - position);
        GameObject shadowman = Instantiate(shadowmanFacePrefab, position, facingPlayer);
        shadowman.GetComponent<AnimationController>().playerTransform = playerTransform;
        // shadowman.GetComponent<Animator>().SetBool("isWalking", true);
        shadowman.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().material.color = Color.gray;
        shadowman.transform.Find("Face").GetComponent<SkinnedMeshRenderer>().material.color = Color.gray;
        shadowman.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        shadowman.transform.Find("Face").GetComponent<SkinnedMeshRenderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        shadowmen.Add(shadowman);
    }

    void OnEnable() {
        devGun.OnObjectPlaced += HandleObjectPlaced;
        // phases[11].Summon();
    }

    void OnDisable() {
        devGun.OnObjectPlaced -= HandleObjectPlaced;
    }

    void HandleObjectPlaced(GameObject placedObject) {
        if (devGun.step == -1)
        {
            this.placedObject = placedObject;
        }
        else if (devGun.step == 0)
        {
            placedDoor = placedObject;
        }
        else if (devGun.step == 1)
        {
            placedBigDoor = placedObject;
        }
        else if (devGun.step == 3)
        {
            directionToggle *= -1;
            ImpOrbitData data = new ImpOrbitData
            {
                radius = Random.Range(0.5f, 2.5f),
                speed = Random.Range(3f, 4f),
                startPos = Random.Range(0, 2 * Mathf.PI),
                direction = directionToggle,
                centre = placedObject.transform.position 
                + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), 
                tilt = Random.Range(-30f, 30f),
                squeeze = Random.Range(0f, 0.5f)
            };
            imps[placedObject] = data;

        }
        else if (devGun.step == 4)
        {
            placedBanshee = placedObject;
        }
        else if (devGun.step == 5)
        {
            placedCrawler = placedObject;
        }
    }

    public void EnterNextPhase()
    {

        print("Entering phase " + currentPhaseIndex);
        if (phases.TryGetValue(currentPhaseIndex, out GamePhase currentPhase))
        {
            currentPhase.ExitPhase();
        }

        currentPhaseIndex++;
        if (phases.TryGetValue(currentPhaseIndex, out GamePhase nextPhase))
        {
            nextPhase.EnterPhase();
        }
        else
        {
            Debug.Log("End of phase sequence or invalid phase index.");
        }
    }

    IEnumerator ResetButtonPress()
    {
        yield return new WaitForSeconds(0.5f);
        canPressButton = true; // Re-enable button press
    }

    public IEnumerator ChangeBrightness(float endBrightness, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            passthroughLayer.SetBrightnessContrastSaturation(Mathf.Lerp(currentBrightness, endBrightness, t), 0, 0);
            //lightSource.intensity = Mathf.Lerp(1 - startBrightness, 1 - endBrightness, t);
            time += Time.deltaTime;
            yield return null;
        }
        currentBrightness = endBrightness;
        // passthroughLayer.SetBrightnessContrastSaturation(endBrightness, 0, 0);
        //lightSource.intensity = 1 - endBrightness;
    }

    public IEnumerator FadeInVolume(AudioSource source, float duration)
    {
        float currentTime = 0;
        source.volume = 0;
        source.Play();
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(0, 1, currentTime / duration);
        }
        source.volume = 1;
        yield return null;
    }

    public IEnumerator StopAudioAfterTime(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        source.Stop();
    }

    public void SpawnShadowFace()
    {
        shadowmanFace = Instantiate(shadowmanFace, placedObject.transform.position, placedObject.transform.rotation);  
        // summmon many copies
        shadowmanFace.SetActive(true);
        Debug.Log("Shadowman face spawned"); 
    }
    public void InvokeEnableWallParent(bool enable, float delay)
    {
        if(enable)
        {
            Invoke("EnableWallParent", delay);
        }
        else
        {
            Invoke("DisableWallParent", delay);
        }
    }

    void EnableWallParent()
    {
        wallRotator.SetWallParentActive(true);
    }

    void DisableWallParent()
    {
        wallRotator.SetWallParentActive(false);
    }

    public void ExtendWall()
    {
        wallRotator.ExtendRoomDownwards();
    }

    public void DisableSkyboxLighting()
    {
        RenderSettings.ambientIntensity = 0f;
    }

    public void print (string message)
    {
        debugText.text = message;
    }

    public struct ImpOrbitData
    {
        public float radius;
        public float speed;
        public float startPos;
        public int direction;
        public Vector3 centre;
        public float tilt;
        public float squeeze;
    }
}

