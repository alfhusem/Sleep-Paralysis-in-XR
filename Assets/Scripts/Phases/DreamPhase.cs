using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamPhase : GamePhase
{
    public DreamPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.video.SetActive(true); //Already active?
        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {
        manager.camera.clearFlags = CameraClearFlags.Skybox; // TODO: add transition
        RenderSettings.skybox = manager.dreamSkybox;
        
        // manager.FadeInVolume(manager.dreamAudioSource, 2);
        manager.dreamAudioSource.Play();

        foreach (var imp in manager.impsList)
        {
            // PlaceObject(imp, cursorpos);
            // Instantiate(imp);
            manager.SummonImp(imp);

        }  

        yield return new WaitForSeconds(10);

        manager.impsFlyAway = true; // Fly away gradually

        yield return new WaitForSeconds(7); // 10
        manager.DestroyImps();

        manager.lovecraft.SetActive(true);
        Vector3 directionToPlayer = (manager.placedBigDoor.transform.position - manager.playerTransform.position).normalized;
        manager.lovecraft.transform.position = manager.playerTransform.position + directionToPlayer * 1000;
        manager.lovecraft.transform.position = new Vector3(manager.lovecraft.transform.position.x, -100, manager.lovecraft.transform.position.z);

   
        
        yield return new WaitForSeconds(2f);
        
        // manager.EnterNextPhase(); // This happens in manager.FixedUpdate
    }

    public override void ExitPhase()
    {
        // manager.camera.clearFlags = CameraClearFlags.SolidColor;
        // manager.passthroughLayer.enabled = false; // Check
        RenderSettings.skybox = manager.blackSkybox;
        manager.StartCoroutine(manager.ChangeBrightness(-0.5f, 0));
        manager.dreamAudioSource.Stop();
    }

}

