using System.Collections;
using UnityEngine;

public class KnockingPhase : GamePhase
{
    public KnockingPhase(GamePhaseManager manager) : base(manager) { }

    public override void EnterPhase()
    {
        manager.StartCoroutine(PhaseSequence()); 
    }

    private IEnumerator PhaseSequence()
    {

        yield return manager.ChangeBrightness(-0.5f, 5);

        yield return new WaitForSeconds(5);

        // Get the knocking audio source and play it
        AudioSource knockingAudio = manager.placedDoor.transform.GetChild(0).GetComponent<AudioSource>();
        knockingAudio.Play();

        yield return new WaitForSeconds(8f);

        AudioSource whisperInEarAudio = manager.whisperInEarAudioSource.GetComponent<AudioSource>();
        whisperInEarAudio.Play();

        yield return new WaitForSeconds(5f);

        yield return manager.StopAudioAfterTime(knockingAudio, 0);
        // yield return manager.StopAudioAfterTime(behindYouAudio, 0);

        // Transition to the next phase
        manager.EnterNextPhase();
    }
}
