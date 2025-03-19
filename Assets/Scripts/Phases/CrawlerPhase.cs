using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerPhase : GamePhase
{
    public CrawlerPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {

        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {   
        yield return new WaitForSeconds(4);
        manager.camera.clearFlags = CameraClearFlags.SolidColor;

        manager.placedCrawler.SetActive(true);

        yield return new WaitForSeconds(2);

        manager.horrorAudioSource.Play();

        yield return new WaitForSeconds(8f);

        manager.placedCrawler.SetActive(false);

        manager.camera.clearFlags = CameraClearFlags.Skybox; // Turn to black
        
        manager.EnterNextPhase();

    }

}

