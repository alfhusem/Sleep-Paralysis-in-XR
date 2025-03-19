using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatPhase : GamePhase
{
    private GameObject placedObject;

    public BatPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.StartCoroutine(PhaseSequence());
    }

    private IEnumerator PhaseSequence()
    {
        manager.batsAudioSource.Play();

        foreach (var imp in manager.imps)
        {
            imp.Key.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

        yield return new WaitForSeconds(10);

        manager.batsAudioSource.Stop();

        //More bats
        
        manager.EnterNextPhase();

    }

}
