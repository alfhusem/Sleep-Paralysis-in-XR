using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPhase : GamePhase
{
    private DevGun devGun;

    public SetupPhase(GamePhaseManager manager, DevGun devGun) : base(manager)
    {
        this.devGun = devGun;
    }

    public override void EnterPhase()
    {
        
    }

    public override void ExitPhase() 
    {
        devGun.enabled = false; // Ensure the DevGun is disabled when leaving the setup phase
    }
    
}
