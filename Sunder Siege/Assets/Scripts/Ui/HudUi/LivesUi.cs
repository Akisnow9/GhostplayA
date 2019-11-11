using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUi : MonoBehaviour
{
    [SerializeField] private Text m_livesDisplay;
    [SerializeField] private List<LivesSprite> m_spriteList;
    [SerializeField] private List<GameObject> m_labelsToTurnOff;
    [SerializeField] private float m_timeBetweenSpriteChange;
    private int m_spritesDisplayed;
    private bool m_overflow = true; // If the the textbox showing overflow exitst

    private void Awake()
    {
        m_spritesDisplayed = m_spriteList.Count;
    }


    void LateUpdate()
    {
        int lives = Timer.LivesGet(); // Get lives
        if (m_overflow)
        {
            if (lives > m_spriteList.Count)                                    // Checks if lives is over the displayed amount of sprites.
            {
                m_livesDisplay.text = (lives - m_spriteList.Count).ToString(); // Will display how many lives minue the amount of sprites.  
            }
            else
            {
                foreach(GameObject label in m_labelsToTurnOff)                      // Disable ui elemnts here.
                {
                    label.SetActive(false);
                }
                m_overflow = false;                                            // Will stop coming into this loop now.
            }
        }
        else
        {
            if(lives < m_spritesDisplayed)
            {
                //cycle enable sprites cycle/animation. Will be it's own script.
                m_spriteList[m_spritesDisplayed - 1].Setup(m_timeBetweenSpriteChange); // It does thing above.
                m_spritesDisplayed--;
            }
        }
    }

}

