using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemIndicator : MonoBehaviour
{
    //[SerializeField] private float m_pop;
    //[SerializeField] private float m_popEvery;


    private float m_lastPop;
    private float m_timeBetweenPulse = 1.0f;

    private float m_lastPulse;

    private float m_max = 0.030f;
    private float m_steps = 0.001f;

    private float m_startScale;

    private bool m_doPulse = false;
    private bool m_maxReached = false;
    // Update is called once per frame


    private void Start()
    {
        m_startScale = this.transform.localScale.x;
    }
    void LateUpdate()
    {
        this.transform.LookAt(Camera.main.transform);

        if(m_doPulse)
        {
            DoPulse();
        }
        else if(m_lastPulse < Time.time)
        {
            m_doPulse = true;
            DoPulse();
        }

    }
    public void Setup(GameObject a_problemToSpawnOver, Vector3 a_uiPostion)
    {
        this.transform.position = a_problemToSpawnOver.transform.position + a_uiPostion;
        m_lastPulse = Time.time + m_timeBetweenPulse;
    }

    private void DoPulse()
    {
        if(m_maxReached)
        {
            // Scales Vector down.
            this.transform.localScale = this.transform.localScale - new Vector3(m_steps, m_steps, m_steps);
            if(this.transform.localScale.x <= m_startScale)
            {
                this.transform.localScale = new Vector3(m_startScale, m_startScale, m_startScale);
                m_maxReached = false;
                m_doPulse = false;
                m_lastPulse = Time.time + m_timeBetweenPulse;
            }

        }
        else
        {
            this.transform.localScale = this.transform.localScale + new Vector3(m_steps, m_steps, m_steps);
             if(this.transform.localScale.x >= m_max)
            {
                m_maxReached = true;
            }
        }
    }

}
