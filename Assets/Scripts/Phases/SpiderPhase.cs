using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPhase : GamePhase
{
    public SpiderPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {

        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {   
        yield return new WaitForSeconds(4);
        manager.camera.clearFlags = CameraClearFlags.SolidColor;

        yield return new WaitForSeconds(5);

        float minX = Mathf.Min(manager.corners[0].x, manager.corners[1].x, manager.corners[2].x, manager.corners[3].x);
        float maxX = Mathf.Max(manager.corners[0].x, manager.corners[1].x, manager.corners[2].x, manager.corners[3].x);
        float minZ = Mathf.Min(manager.corners[0].z, manager.corners[1].z, manager.corners[2].z, manager.corners[3].z);
        float maxZ = Mathf.Max(manager.corners[0].z, manager.corners[1].z, manager.corners[2].z, manager.corners[3].z);
        manager.spiderStart = manager.wallRotator.height;

        for (int i = 0; i < 25; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, manager.spiderStart, randomZ);
            GameObject spawnedSpider = Object.Instantiate(manager.spider, spawnPosition, Quaternion.Euler(45, Random.Range(0, 360), -45));
            manager.spiders.Add(spawnedSpider);
            yield return new WaitForSeconds(Random.Range(0.3f, 1f));
        }
        
        yield return new WaitForSeconds(3);
        manager.EnterNextPhase();

    }

    public override void ExitPhase() {
        manager.camera.clearFlags = CameraClearFlags.Skybox;
        foreach (GameObject spider in manager.spiders)
        {
            Object.Destroy(spider);
        }
    }

}
