using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkForestPhase : GamePhase
{
    public DarkForestPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {
        yield return new WaitForSeconds(4);
        RenderSettings.skybox = manager.darkForestSkybox;
        manager.crazyWhispersAudioSource.Play();

        yield return new WaitForSeconds(4);

        // spawn spiders
        int max = 18;
        manager.spiderFloor = -100;
        manager.spiderStart = 25f;
        manager.spiderSpeed = 1.5f;
        for (int i = 0; i < 100; i++)
        {
            float randomX = Random.Range(-max, max);
            float randomZ = Random.Range(-max, max);
            Vector3 spawnPosition = new Vector3(randomX, manager.spiderStart - Random.Range(0f, 5f), randomZ);
            GameObject spawnedSpider = Object.Instantiate(manager.bigSpider, spawnPosition, Quaternion.Euler(45, Random.Range(0, 360), -45));
            manager.spiders.Add(spawnedSpider);
        }
         

        yield return new WaitForSeconds(15);


        // summon shadowmen (some with faces, some without)
        int count = 10;
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            float radius = UnityEngine.Random.Range(38, 40f);
            float x = manager.playerTransform.position.x + Mathf.Cos(angle) * radius;
            float z = manager.playerTransform.position.z + Mathf.Sin(angle) * radius;
            Vector3 position = new Vector3(x, -7, z);
            manager.SummonShadowman(position);
            yield return new WaitForSeconds(0.002f);
        }

        // destroy spiders
        foreach (GameObject spider in manager.spiders)
        {
            Object.Destroy(spider);
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(2); // 10
        
        // start walking
        foreach (var shadowman in manager.shadowmen)
        {
            shadowman.GetComponent<Animator>().SetBool("isWalking", true);
            yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        }

        // stop walking
        // handled in manager.Update

     
        yield return new WaitForSeconds(20f);

        foreach (var shadowman in manager.shadowmen)
        {
            shadowman.GetComponent<Animator>().SetBool("isRunning", true);
        }
                
        // manager.EnterNextPhase(); -- handled in manager.Update
    }

    public override void ExitPhase()
    {
        RenderSettings.skybox = manager.blackSkybox;
        manager.crazyWhispersAudioSource.Stop();
        
        foreach (var shadowman in manager.shadowmen)
        {
            Object.Destroy(shadowman);
        }
    }

}

