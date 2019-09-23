using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TimeUi : MonoBehaviour
{
    [SerializeField] private Text m_timeDisplay;

    void LateUpdate()
    {
        int displayTime = (int)Timer.TimeGet();
        m_timeDisplay.text = displayTime.ToString();
    }

}
