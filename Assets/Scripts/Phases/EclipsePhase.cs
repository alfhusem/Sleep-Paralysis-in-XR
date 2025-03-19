using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EclipsePhase : GamePhase
{
    
    public EclipsePhase(GamePhaseManager manager) : base(manager)
    {}

    public override void EnterPhase()
    {
        manager.StartCoroutine(PhaseSequence());
    }

    private IEnumerator PhaseSequence() {

        // Starts off dark
        yield return new WaitForSeconds(4);
        manager.camera.clearFlags = CameraClearFlags.SolidColor;
        yield return manager.ChangeBrightness(-0.3f, 6);
        yield return new WaitForSeconds(1);

        manager.InvokeEnableWallParent(true, 1); 

        manager.eclipseAudioSource.Play();

        yield return new WaitForSeconds(1f); // wait for InvokeEnableWallParent to finish
        manager.placedDoor.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        manager.placedDoor.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;

        yield return new WaitForSeconds(10f);

        // Knocking sounds

        AudioSource hardKnockAudio = manager.placedDoor.transform.GetChild(0).GetChild(0).GetComponent<AudioSource>();
        AudioSource franticKnockAudio = manager.placedDoor.transform.GetChild(0).GetChild(1).GetComponent<AudioSource>();
        AudioSource doorOpenAudio = manager.placedDoor.transform.GetChild(0).GetChild(2).GetComponent<AudioSource>();
        franticKnockAudio.Play(); // Merge with StopAudioAfterTime?
        yield return new WaitForSeconds(15f);
        franticKnockAudio.Stop();
        yield return new WaitForSeconds(4f);
        hardKnockAudio.Play(); // Merge with StopAudioAfterTime?
        yield return manager.StopAudioAfterTime(hardKnockAudio, 2); // AudioDuration=2

        yield return new WaitForSeconds(5f);

        Vector3 directionToPlayer = (manager.placedDoor.transform.position - manager.playerTransform.position).normalized;
        directionToPlayer.y = 0; 
        manager.SpawnShadowFace();
        manager.shadowmanFace.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        manager.shadowmanFace.transform.Find("Face").GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        manager.shadowmanFace.transform.position = manager.placedDoor.transform.position + directionToPlayer * 2;
        manager.shadowmanFace.transform.rotation = manager.placedDoor.transform.rotation;

        doorOpenAudio.Play();

        yield return manager.OpenDoor(manager.placedDoor, 120, 20, 0.4f);

        yield return new WaitForSeconds(6f);
        
        var objectAnimator = manager.shadowmanFace.GetComponent<Animator>();
        objectAnimator.SetBool("isCrawling", true);
        yield return new WaitWhile(() => objectAnimator.GetBool("isCrawling"));

        manager.ExtendWall();
        yield return new WaitForSeconds(0.2f);
        EnableGravity(true);

        // --- manager.EnterNextPhase(); This happens in manager.FixedUpdate
    }

    public override void ExitPhase() 
    {
        // manager.InvokeEnableWallParent(false, 1);
        manager.placedDoor.SetActive(false);
    }

    public void EnableGravity(bool enable) 
    {
        manager.gravityEnabled = enable;
        manager.fallingCamera.SetActive(enable);
        manager.fallingCamera.transform.position = manager.playerTransform.position;
    }
    
}
