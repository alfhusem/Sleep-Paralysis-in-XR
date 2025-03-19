using UnityEngine;

public class PassthroughManager : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer passthroughLayer1;

    // private int beep = 0;

    // void Update()
    // {
    //     if (OVRInput.GetDown(OVRInput.Button.One))
    //     {
    //         beep++;
    //         if (beep == 2)
    //         {
    //             beep = 0;
    //         }
    //     }

    //     float triggerPressure = -OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
    //     if (beep == 0) {
    //         passthroughLayer1.SetBrightnessContrastSaturation(-1, -triggerPressure, 0);
    //     }
    //     else if (beep == 1) {
    //         passthroughLayer1.SetBrightnessContrastSaturation(-1, triggerPressure, 0);
    //     }

       // if (OVRInput.GetDown(OVRInput.Button.One))
        // {
        //     lightSource.enabled = !lightSource.enabled;
        // }
        // float triggerPressure = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        // if (lightSource.enabled) {
        //     lightSource.intensity = Mathf.Lerp(0, 100, triggerPressure);
        // }

    // }

}
