/***************************************************
 * Written By: Anton Huber
 * Purpose: Stores math related to ui and calls funciton 
 * to inccrement/ decrement sprites in appropriate class
 * Data Created: 8/10
 * Last Modified: 8/10
 **************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//THIS CLASS IS UI IT NEEDS TO BE RENAMED
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



public class ProblemController : MonoBehaviour
{
    //*************************************************************************************
    // Handles spawning of the ui elements and math.
    //*************************************************************************************

    private int m_maxButtonPresses;
    private float m_startTime;


    private Problem m_problemToSpawnOver;
    [SerializeField]private BaseUi m_progressSprite;
    [SerializeField]private BaseUi m_timeSprite;
    
    //[SerializeField]private float m_spawnDistance;

    // Does this need effects animations?
    // Scales? Oppacity?
    // private float 

    // Start is called before the first frame update
    private void Awake()
    {
        this.transform.LookAt(Camera.main.transform); // Currently shows back of sprite rather then front
        ResetSprites();   // Activate and deactivate all peices of ui
    }
   
    void Update()
    {
        this.transform.LookAt(Camera.main.transform); // Currently shows back of sprite rather then front
        this.transform.Rotate(0, 180, 0); // Look at seems to look exactley away. Could be a problem with the prefab.
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    m_timeSprite.Decrement();
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    m_timeSprite.Increment();
        //}

        float number = (Timer.TimeGet() - m_problemToSpawnOver.GetTime()) / (m_startTime - m_problemToSpawnOver.GetTime());
        float number2 = ((float)m_timeSprite.GetActiveSprites() / (float)m_timeSprite.m_orderedSpriteList.Count); // I've done this due to having some issues with data loss. It can be done in the line below but.
        if (m_timeSprite.GetActiveSprites() != 0)
        {
            if (number2 > number) // I was having a loss of data issue. They all need to be floats otherwise loss of data
            {
                m_timeSprite.Decrement();
            }
        }
        if (m_progressSprite.GetActiveSprites() != 0) // So you don't divide by 0;
        {
            number = (float)m_problemToSpawnOver.GetButtonPressCount() / (float)m_problemToSpawnOver.GetButtonPressesMax();
            number2 = (float)m_progressSprite.GetActiveSprites() / (float)m_progressSprite.m_orderedSpriteList.Count;
            if (number > number2)
            {
                m_progressSprite.Increment();
            }
        }
        else if (m_problemToSpawnOver.GetButtonPressCount() != 0) // Since start is 0 need a special case.
        {
            m_progressSprite.Increment();
        }
    }

   

    public void Setup(Problem a_problemToSpawnOver, Vector3 a_uiPostion)
    {
        this.transform.SetParent(a_problemToSpawnOver.transform, false);
        m_problemToSpawnOver = a_problemToSpawnOver;
        //this.transform.position = m_problemToSpawnOver.transform.position + new Vector3(0, m_spawnDistance, 0);
        this.transform.position = m_problemToSpawnOver.transform.position + a_uiPostion;
        this.gameObject.SetActive(false);
    }

    public void Activate()
    {
        // Needs to call all ui sprites and turn them all on
        // Takes the time that has been adjusted and check against
        this.gameObject.SetActive(true);
        m_maxButtonPresses = m_problemToSpawnOver.GetButtonPressesMax();
        m_startTime = Timer.TimeGet();
    }

    public void ResetSprites()
    {
        m_timeSprite.ResetPeices();
        m_progressSprite.ResetPeices();
        // Resets the ui to be how it should be on activate.
        foreach (GameObject sptire in m_timeSprite.m_orderedSpriteList)
        {
            m_timeSprite.Increment();
        }
        foreach (GameObject sprite in m_progressSprite.m_orderedSpriteList)
        {
            m_progressSprite.Deactivate();
        }
    }
}
