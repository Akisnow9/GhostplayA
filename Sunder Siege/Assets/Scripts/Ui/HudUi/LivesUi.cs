﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUi : MonoBehaviour
{
    [SerializeField] private Text m_timeDisplay;



    void LateUpdate()
    {
        m_timeDisplay.text = Timer.LivesGet().ToString();
    }

}
