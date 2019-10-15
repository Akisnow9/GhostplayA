/***************************************************
 * Written By: Anton Huber
 * Purpose: A sprite list and common functions for ui
 * Data Created: 9/10
 * Last Modified: 9/10
 **************************************************/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUi : MonoBehaviour
{
    //*************************************************************************************
    // Common functions like increment and decrement as well as sprite lists.
    // All spawned ui elemnts will need to inherit from this
    // 
    //*************************************************************************************

    public List<GameObject> m_orderedSpriteList; // All ui sprites.
    private int m_activePieces = 0;

    // Start is called before the first frame update

    public void Deactivate()
    {
        // All of the sprites are now deactivated.
        foreach (GameObject sprite in m_orderedSpriteList)
        {
            sprite.SetActive(false);
        }
    }

    public bool Increment() // Could be changed to voids
    {
        m_orderedSpriteList[m_activePieces].SetActive(true);
        m_activePieces++;
        return m_activePieces == m_orderedSpriteList.Count; // Return that it has reached the max sprites
    }

    public bool Decrement() // Could be changed to voids
    {
        m_orderedSpriteList[m_activePieces - 1].SetActive(false);
        m_activePieces--;
        return m_activePieces == 0;
    }

    public int GetActiveSprites()
    {
        return m_activePieces;
    }
    public void ResetPeices()
    {
        m_activePieces = 0;
    }
}
