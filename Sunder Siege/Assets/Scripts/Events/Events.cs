/***************************************************
 * Written By: Anton Huber
 * Purpose: Contains enum list for triggerable events.
 * Data Created:15/09
 * Last Modified: 15/09
 **************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//*************************************************************************************
// Events created by level designer.
// Events will need to be seperate game objects in this iteration of the event system.
//*************************************************************************************
// THINGS TO ADD - 
// HOW MANY TIMES IT IS REPEATED. - Should be an int could have a bool as well to say till game end.
//*************************************************************************************




// This is a waste of time.
// I'm sure it could be done better.
public class Events : MonoBehaviour
{
    public E_Event m_event; // The actual queued event.
    public float m_eventTriggerTime; // This is the time the event will trigger. Should be less then current time or will fire instantly 
    public int m_dataToModify; // The value that is passed to the function to adjust the values

    public bool m_repeatable; // Is the event repeatable?
    public float m_repeatEvery; // If so how ofter?
    [SerializeField] private float m_minRange = 0; // Minus this from max. Then add to m_repeatEvery. 
    [SerializeField] private float m_maxRange = 0; // As above. Note that should be left zero if not range required.

    // Needs a variable if there is a limit to the amount of repeats. Could be another bool
    // Variable that gets decremented if so. Upon '0' should be removed.

    // These are mostly redundant but may have some use if I can change the class to.. uhh.. scripted objets? Not neccesary atm.
    
    public Events(E_Event a_Event, float a_time, int a_modifiedData)
    {
        m_event = a_Event;
        m_eventTriggerTime = a_time;
        m_dataToModify = a_modifiedData;
        m_repeatable = false;
        m_repeatEvery = 0;
    }

    public Events(E_Event a_Event, float a_time, int a_modifiedData, float a_repeatEvery)
    {
        m_event = a_Event;
        m_eventTriggerTime = a_time;
        m_dataToModify = a_modifiedData;
        m_repeatable = true;
        m_repeatEvery = a_repeatEvery;
    }

    public float GetRange()
    {
        //Just does quick math to get a number between the range. To then be applied when the event is repeatable.
        return Random.Range(0, m_maxRange) - m_minRange;
    }
}

