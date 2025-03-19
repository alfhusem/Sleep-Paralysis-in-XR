using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public OVRPassthroughLayer passthroughLayer; // Assign this in the inspector
    // private int currentPhase = -1;
    // private bool canChangePhase = true;
    // int m = 0;

    void Update()
    {
        // Check if the button is pressed
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (director.state != PlayState.Playing)
            {
                director.time = 0;
            }

            StartCoroutine(ChangeBrightness());
            director.Play();

            // if (canChangePhase)
            // {
            //     currentPhase++;
            //     Debug.Log("Changed to phase " + currentPhase);
            //     canChangePhase = false; // Prevent changing phase again until it's allowed
            //     ProcessPhase();
            // }
            // else {
            //     Debug.Log("Cant. Phase " + currentPhase);
            // }
        }
        
        // if (OVRInput.GetDown(OVRInput.Button.Two)) {
        //     m = m == 0 ? 1 : 0;
        //     passthroughLayer.SetBrightnessContrastSaturation(m, 0, 0);
        // }
    }

    // void ProcessPhase()
    // {
    //     switch (currentPhase)
    //     {
    //         case 0:
    //             // Idle

    //             StartCoroutine(ChangeBrightness());

    //             director.Play();
    //             canChangePhase = true;
    //             Debug.Log("Phase " + currentPhase +"(0) done");
    //             break;
    //         case 1:
    //             // Start walking animation
    //             //StartCoroutine(SmoothTransition(701, 2));
    //             SetTime(700);
    //             Debug.Log("Phase " + currentPhase + "(1) done");
    //             // waiting for signal
    //             currentPhase = -1;
    //             break; 
 
    //     }
    // }

    // if you have signals set in your timeline to indicate the end of an animation:
    // public void OnSignalReceived()
    // {
    //     canChangePhase = true; // Allow changing phase when signal is received
    // }

    public void SetTime(float time)
    {
        // float duration = 1f;
        // if (duration == 0f){
            director.time = time;
            director.Evaluate();
        // }
        // else {
        //     StartCoroutine(SmoothTransition(time, duration));
        // }
        // StartCoroutine(SmoothTransition(time, 1f));
        
    }

    IEnumerator SmoothTransition(float targetTime, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        float startTimelineTime = (float) director.time;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration; // Normalize time to 0-1
            director.time = Mathf.Lerp(startTimelineTime, targetTime, t);
            director.Evaluate();
            yield return null; // Wait for the next frame
        }

        director.time = targetTime; // Ensure it ends exactly on the target time
        director.Evaluate();
    }

    public IEnumerator ChangeBrightness()
    {
        // Gradually increase brightness to 1 over 1 second
        float duration = 2.0f; // Duration in seconds
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            passthroughLayer.SetBrightnessContrastSaturation(-t, 0, 0);
            yield return null; // Wait for next frame
        }

        // Ensure brightness is set to 1 at the end of the interval
        passthroughLayer.SetBrightnessContrastSaturation(-1, 0, 0);

        // Gradually decrease brightness back to 0 over another second
        startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            passthroughLayer.SetBrightnessContrastSaturation(-1 + t, 0, 0);
            yield return null; // Wait for next frame
        }
        // Ensure brightness is set to 0 at the end of the interval
        passthroughLayer.SetBrightnessContrastSaturation(0, 0, 0);
    }

}
