using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadRotation : MonoBehaviour
{
    // Meant to detect if it should rotate towards an object in front of it

    [SerializeField] private GameObject m_joint;


    void Update()
    {
        if (true) //The range of the check goes here or is calculated here via a funciton that returns a bool or checks if null.
        {
            // Will need the object that the character is turning towards here.Or at the very least the position of it.
        }
        else
        {
            // Will need 


        }
    }

    private void CheckDistance() // Checks if object within a cone of the players body. -- Might return a bool or some reference to the object or retrun null.
    {

    }

}
