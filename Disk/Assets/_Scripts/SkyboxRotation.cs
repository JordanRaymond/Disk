using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour {

    public float speedFactor = 1;

	void Update () {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speedFactor);
    }
}
