using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPhase : GamePhase
{
    private GameObject placedObject;
    public StartPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.InvokeEnableWallParent(false, 1); // Disable the wall parent object
        placedObject = manager.placedObject;
        placedObject.SetActive(false);
        manager.placedDoor.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;//make door invisble
        manager.placedDoor.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        manager.placedBigDoor.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        manager.placedBigDoor.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;

        manager.playerTransform.Find("Canvas").gameObject.SetActive(false);
        
        manager.DisableSkyboxLighting();
        // foreach (var imp in manager.imps)
        // {
        //     imp.Key.transform.Find("mesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
        // } // Handled in DevGun
        manager.placedBanshee.SetActive(false);
        manager.placedCrawler.SetActive(false);   
        Debug.Log("Start Phase ended");     
    }

}
