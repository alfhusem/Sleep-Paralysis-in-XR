using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorPhase : GamePhase
{
    public CorridorPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {
        

        manager.wallRotator.floor.SetActive(true);
        manager.placedBigDoor.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        manager.placedBigDoor.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;



        manager.shadowmanFace.SetActive(true);
        Vector3 doorPosition = manager.placedBigDoor.transform.position;
        Vector3 directionToPlayer = (doorPosition - manager.playerTransform.position).normalized;
        directionToPlayer.y = 0; 
        manager.shadowmanFace.transform.position = doorPosition + manager.placedBigDoor.transform.forward * 2;//directionToPlayer * 2;
        
        manager.shadowmanFace.transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        var renderer = manager.shadowmanFace.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>();
        var rendererFace = manager.shadowmanFace.transform.Find("Face").GetComponent<SkinnedMeshRenderer>();
        var objectAnimator = manager.shadowmanFace.GetComponent<Animator>();
        objectAnimator.SetTrigger("goIdle");
        
        yield return new WaitForSeconds(5f);

        AudioSource hardKnockAudio = manager.placedBigDoor.transform.GetChild(0).GetChild(0).GetComponent<AudioSource>();
        hardKnockAudio.Play();
        yield return new WaitForSeconds(4f);
        hardKnockAudio.Stop();
        yield return new WaitForSeconds(5f);


        // Open door
        yield return manager.OpenDoor(manager.placedBigDoor, 180, 500, 0.5f);
        manager.placedBigDoor.transform.GetChild(0).GetChild(3).GetComponent<AudioSource>().Play();

        // Shadowman is visible

        renderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        rendererFace.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        renderer.material.color = Color.gray;
        rendererFace.material.color = Color.gray;


        yield return new WaitForSeconds(5f);

        objectAnimator.SetBool("isWalking", true);
        yield return new WaitForSeconds(3.5f); 
        

        objectAnimator.SetBool("isRunning", true);
        yield return new WaitWhile(() => objectAnimator.GetBool("isRunning"));
        manager.shadowmanFace.SetActive(false);
        manager.EnterNextPhase();
    }

    public override void ExitPhase()
    {
        manager.InvokeEnableWallParent(false, 0);
        manager.placedBigDoor.SetActive(false);

        manager.camera.clearFlags = CameraClearFlags.Skybox;
        // manager.passthroughLayer.enabled = false; // Check
        manager.ChangeBrightness(-0.5f, 0);
    }

}

