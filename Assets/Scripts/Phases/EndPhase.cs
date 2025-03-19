using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPhase : GamePhase
{
    public EndPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.StartCoroutine(PhaseSequence());
    }

    private IEnumerator PhaseSequence()
    {
        yield return new WaitForSeconds(3);
        manager.camera.clearFlags = CameraClearFlags.SolidColor;
        yield return manager.ChangeBrightness(0, 3);


    }

}
