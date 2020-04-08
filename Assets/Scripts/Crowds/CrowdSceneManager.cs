﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.white;
        RenderSettings.fogDensity = 0.003f;
        CCPlayer.main.useGravity = false;
        CCPlayer.main.SetActiveHandType(HandType.Regular);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
