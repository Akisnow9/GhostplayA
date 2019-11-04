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

    [SerializeField] private float m_elemnenFadeSpeed = 0.05f;

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
        m_orderedSpriteList[m_activePieces].SetActive(true); // Change to function on the object which will increase opactiy. -- This will now happen in fadein.
       // m_orderedSpriteList[m_activePieces - 1].GetComponent<ProblemUiElementFade>().FadeOut(m_elemnenFadeSpeed);
        m_activePieces++;
        return m_activePieces == m_orderedSpriteList.Count; // Return that it has reached the max sprites
    }

    public bool Decrement() // Could be changed to voids
    {
        m_orderedSpriteList[m_activePieces - 1].SetActive(false); // Change to function on the object which will reduce opactiy. -- This will now happen in fafeout.
        //m_orderedSpriteList[m_activePieces - 1].GetComponent<ProblemUiElementFade>().FadeOut(m_elemnenFadeSpeed);
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
