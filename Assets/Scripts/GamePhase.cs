using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GamePhase
{
    protected GamePhaseManager manager;

    public GamePhase(GamePhaseManager manager)
    {
        this.manager = manager;
    }

    public abstract void EnterPhase();

    public virtual void ExitPhase()
    {
        // Default implementation can be empty
    }

}


