using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemIndicator : MonoBehaviour
{
    //[SerializeField] private float m_pop;
    //[SerializeField] private float m_popEvery;


    private float m_lastPop;

    

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(Camera.main.transform);
    }
    public void Setup(GameObject a_problemToSpawnOver, Vector3 a_uiPostion)
    {
        this.transform.position = a_problemToSpawnOver.transform.position + a_uiPostion;
    }

}
