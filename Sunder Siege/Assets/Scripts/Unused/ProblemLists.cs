/***************************************************
 * Written By: Anton Huber
 * Purpose: Centralized place for the active and inactive lists and associated functions
 * Data Created:14/09
 * Last Modified: 14/09
 **************************************************/

//*************************************************************************************
//*************************************************************************************
// THIS IS BEING KEPT AS A PROOF OF CONCEPT OF A ALTERNATIVE IMPLEMENTATION.         //
// IT IS NOT REFERENCED IN THE CODE.                                                 //
//*************************************************************************************
//*************************************************************************************


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemLists : MonoBehaviour
{
    //*************************************************************************************
    // Will contain Lists that store problems.
    //*************************************************************************************

    public List<Problem> m_activeList;
    public List<Problem> m_inactiveList;

    // Returns a bool if an activeproblem reaches the time limit.
    public bool CheckActive(float a_timeLimit)
    {
        //@Checks the state of currently activated objects. If one has expired return true.
        // May run into issues with more than 1 failing at a time. >= should fix this issue.
        // Version that is contained within it's own class.
        foreach (Problem problem in m_activeList)
        {
            if (problem.GetTime() >= a_timeLimit)
            {
                problem.Deactivate(); // Removes the item from active list.
                return true;
            }
        }
        return false;
    }

    
}
