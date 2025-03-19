using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BansheePhase : GamePhase
{
    public BansheePhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {

        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {   
        yield return new WaitForSeconds(4);
        manager.camera.clearFlags = CameraClearFlags.SolidColor;

        manager.placedBanshee.SetActive(true);


        yield return new WaitForSeconds(10);

        manager.bansheeScreanAudioSource.Play();

        yield return new WaitForSeconds(0.5f);

        manager.bansheeCloseup.SetActive(true);
        manager.placedBanshee.SetActive(false);

        manager.bansheeCloseup.transform.position = manager.placedBansheeCloseup.transform.position;
        manager.bansheeCloseup.transform.rotation = manager.placedBansheeCloseup.transform.rotation;
        
        yield return new WaitForSeconds(1f);

        manager.bansheeCloseup.SetActive(false);
        manager.camera.clearFlags = CameraClearFlags.Skybox; // Turn to black
        manager.ChangeBrightness(-1f, 0);
        
        manager.EnterNextPhase();

    }

}
