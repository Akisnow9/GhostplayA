using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenPhysicsObject : MonoBehaviour
{
    private float m_timeToDissapear = 3f; // Time till dissapear
    private float m_scaleSpeed = 0.05f;
    bool m_active = false;

    // Update is called once per frame


    void Update()
    {
        if (m_active) // Safety in case not assagined before 1st update
            if (m_timeToDissapear > Timer.TimeGet())
                if (this.transform.localScale.x >= 0) // Compares time
                {
                    // Every frame shrink if 0 destroy object
                    this.transform.localScale -= new Vector3(m_scaleSpeed, m_scaleSpeed, m_scaleSpeed);
                }
                else
                {
                    m_active = false;
                    this.gameObject.SetActive(false);
                }
    }

    public void Setup(float a_timeToDissapear, float a_scaleSpeed)
    {
        m_timeToDissapear = a_timeToDissapear;
        m_scaleSpeed = a_scaleSpeed;
        m_active = true;
    }
}
