using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPhase : GamePhase
{
    private GameObject placedObject;

    public ShadowPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        placedObject = manager.placedObject;
        var audioSource = placedObject.GetComponent<AudioSource>();
        var objectAnimator = placedObject.GetComponent<Animator>();
        manager.StartCoroutine(PhaseSequence(audioSource, objectAnimator)); 
    }

    private IEnumerator PhaseSequence(AudioSource audioSource, Animator objectAnimator)
    {
        yield return manager.ChangeBrightness(-1f, 4);

        manager.camera.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = manager.blackSkybox;
        

        yield return new WaitForSeconds(3);

        Vector3 directionToPlayer = manager.playerTransform.position - placedObject.transform.position;
        directionToPlayer.y = 0;  // This flattens the direction to the horizontal plane
        Quaternion facePlayer = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        placedObject.SetActive(true);
        placedObject.transform.rotation = facePlayer;


        manager.camera.clearFlags = CameraClearFlags.SolidColor;

        yield return new WaitForSeconds(4);

        audioSource.Play();

        yield return new WaitForSeconds(6);

        // Step 4: Set the isWalking animator bool to true
        objectAnimator.SetBool("isWalking", true);

        // Wait until the animation finishes before proceeding
        yield return new WaitWhile(() => objectAnimator.GetBool("isWalking")); // check this
        yield return new WaitForSeconds(4f);
        
        // manager.SpawnShadowFace();
        // placedObject.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().enabled = false;

        // yield return new WaitForSeconds(0.5f);
        // Animator faceAnimator = manager.shadowmanFace.GetComponent<Animator>();
        // faceAnimator.SetTrigger("lookDown");
        objectAnimator.SetTrigger("lookDown");

        manager.screamAudioSource.Play();
        manager.StartCoroutine(manager.StopAudioAfterTime(manager.screamAudioSource, 3f));

        yield return new WaitForSeconds(1f);

        objectAnimator.SetTrigger("goIdle");
        // manager.shadowmanFace.SetActive(false);
        placedObject.SetActive(false);

        manager.camera.clearFlags = CameraClearFlags.Skybox;

        yield return new WaitForSeconds(5);
        yield return manager.ChangeBrightness(-0.5f, 0);

        manager.camera.clearFlags = CameraClearFlags.SolidColor;
        RenderSettings.skybox = manager.blackSkybox;
        
        
        manager.EnterNextPhase();

    }

}
