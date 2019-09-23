﻿/***************************************************
* Written By: Anton Huber, Ciaran Coppell, Eric Brkic
* Purpose: To keep gametime and trigger events
* Data Created:14/09
* Last Modified: 16/09
**************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Timer : MonoBehaviour
{
    //*************************************************************************************
    // When events will occur. 
    // Eg: Once 'm_timeLimit' reaches 'm_eventTriggerTime' there will be 'm_numberOfEvents' 
    // over the next 'm_eventTriggerTime'
    //*************************************************************************************

    [SerializeField] private float m_timeLimit = 60; // The timer will count down from this point. Set from Unity Editor.
    [SerializeField] private float m_timeBeforeFail = 15; // Once a problem has activated how long before the player loses a life.
    [SerializeField] private int m_numberofLives = 5; // How many times items from the active list can expire.
    //[SerializeField] private float m_minRangeOfProblem = 3; // Minus this from max. Then add to repeat every. Only for problems.
    //[SerializeField] private float m_maxRangeOfProblem = 15; // This minus min. Then add to repeat every. Only for problems.


    private float m_timeScale = 1; // This can be used to modify the rate of seconds passing also movement speed and animation. Useful if people want to include a slowdown effect but needs to be included in everything that is effected. Eg movement/ animation will need to be multiplied by this all the time.


    private int m_numOfInactiveProblems; // The amount of problems in the list that are inactive. Easier then checking every inactive amounts in list.

   
    public List<Events> m_pendingEventList; // The event list. Events are added from the editor.

    public List<Problem> m_problemList; // List of problems. Problems added to this list will be randomly selected.


    public List<Player> m_playerList;

    private static Timer instance = null;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_numOfInactiveProblems = m_problemList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        CheckEventsLists();
        if (CheckProblems()) // Returns true if fail state reached. Either timer hit 0 or lives hit 0.
        {
            //Game over state here.
        }
        m_timeLimit -= m_timeScale * Time.deltaTime; // minus the time between frames * the scaler.
    }

    public static Player PlayerGet(int a_index)
    {
        return instance.m_playerList[a_index];
    }


    //Gets time for ui
    public static float TimeGet()
    {
        return instance.m_timeLimit;
    }
    
    public static int LivesGet()
    {
        return instance.m_numberofLives;
    }

    private void CheckEventsLists()// Not actual name. Might become CheckState.
    {
        // @In charge of queing up and starting new problems. 
        if ((int)m_timeLimit % 1 == 0) // Checks every second rather then every frame. // Might have issues since the time passing may skip the 1 second mark.
        {
            int amountOfEvents = m_pendingEventList.Count;
            for (int i = 0; i < amountOfEvents; i++)
            {
                if (m_pendingEventList[i] == null) // A safety check to make sure the list isn't full of nothing. Will get stuck in an endless loop otherwise.
                {
                    m_pendingEventList.Remove(m_pendingEventList[i]); // Changes the event time.
                    --amountOfEvents;
                    --i;
                }
                if (m_pendingEventList[i].m_eventTriggerTime >= m_timeLimit)  // Needs to check what data is contained on the list - The time needs to be checked first.
                {
                    switch (m_pendingEventList[i].m_event)
                    {
                        case E_Event.SetNumberOfLives:
                            SetNumberOfLives(m_pendingEventList[i].m_dataToModify);
                            break;
                        case E_Event.SetTimeBeforeFail:
                            SetTimeBeforeFail(m_pendingEventList[i].m_dataToModify);
                            break;
                        case E_Event.TriggerProblem:
                            TriggerProblem(m_pendingEventList[i]);
                            break;
                    }
                    if (!m_pendingEventList[i].m_repeatable)
                    {
                        m_pendingEventList.Remove(m_pendingEventList[i]); // Changes the event time.
                        --amountOfEvents;
                        --i;
                    }
                    else
                        EventAdjust(m_pendingEventList[i]);
                }
            }
        }
    }
    

    bool CheckProblems()
    {
        // The function will handle all the swapping of states. Will return true if something on the active list 
        // has expired. Need to deduct lives here only. Version that uses a List of problems.
        // It will also check to see if a problem has completed its 'pending' animation

        for(int i = 0; i < m_problemList.Count; i++ )
        //foreach (Problem problem in m_problemList) -- Due to the way i'm modifying the 'm_problemlist' I can't use for each loops. It seems to get information about the loop at some other point rather then say the count in the above for loop.
        {
            if (m_problemList[i].CheckActive(m_timeLimit)) // Checks if a problem has expired. -- Can probably be used more efficently now that list sorts active
            {
                m_numberofLives--; // Subtract a life.
                Problem addToFront = m_problemList[i]; // Copies itself.
                m_problemList.Insert(0, addToFront); // Puts problem to the front of the list.    --    This will be skipped if the problem has a permanent broken state.
                m_problemList.Remove(m_problemList[i]); // Removes itself.
                m_numOfInactiveProblems++;
            }
            //else if (m_problemList[i].CheckPending())                         // Once a pending animation 
            //    m_problemList[i].Activate(m_timeLimit - m_timeBeforeFail);
        }
        return (m_numberofLives <= 0 || m_timeLimit <= 0.0f);
    }



    //*************************************************************************************
    // Event handler
    // Will contain all funcitons for the switch statements. Line 99
    //*************************************************************************************

    private void EventAdjust(Events a_event)
    {
        // @Adjust the event in the queue placeing it in the event queue. Could probably make some of these variables private and have get and set functions.
        a_event.m_eventTriggerTime = m_timeLimit - a_event.m_repeatEvery + a_event.GetRange();
    }

    void TriggerProblem(Events a_events)
    {
        int position = Random.Range(0, m_numOfInactiveProblems);
        // m_problemList[position].Pending();
        m_problemList[position].Activate(m_timeLimit - m_timeBeforeFail);           // May need to be changed to pending -- The rest just makes sure it is not rolled again.
        Problem addtoback = m_problemList[position];                                // Copies what is in the now activated location.
        m_problemList.Remove(m_problemList[position]);                              // Removes the original problem from the list.
        m_problemList.Insert(m_problemList.Count, addtoback);                       // Adds the copied data to the end of the list.
        m_numOfInactiveProblems--;                                                  // Variable keeping track of inactive problems is adjusted.
    }


    void SetNumberOfLives(int a_numOfLives)
    {
        m_numberofLives = a_numOfLives;
    }
    void SetTimeBeforeFail(float a_timeBeforeFail)
    {
        m_timeBeforeFail = a_timeBeforeFail;
    }
}










    //*************************************************************************************
    // This is a rough outline of a class that can handle difficulty changes.
    // The timer will contain a list of sorted events that will be checked if the time 
    // has been met. Then an enum which will be used to call the appropriate function 
    // and then number stored in the list will modify the current value.
    //
    // eg; At the time of '120' function associated with enum 'NumberOfEvents' will be called
    // and set the value with 'n' which will be passed in by a variable in the list.
    //
    // Each item on the list will contain these variables.
    // float m_eventTime // When function gets called.
    // E_ModifyValue m_modifyvalue // Calls funciton.
    // float m_newValue // What gets passed into the function to set it.
    //*************************************************************************************





    // Used to modify how many events occur over a period of time in game.
    // I'm considering creating a seperate class for it so that all the queue events can 
    // be stored on the same list. Will allow for a much easier time of modifying difficulty
    // during gameplay with events that can be changed easier with enums which will dictate 
    // what action will need to be taken.
    //void SetNumberOfEvents(int a_numberOfEvents)
    //{
    //    m_numberOfEvents = a_numberOfEvents;
    //}



