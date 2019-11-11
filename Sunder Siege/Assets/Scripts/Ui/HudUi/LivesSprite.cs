using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesSprite : MonoBehaviour
{
    [HideInInspector]
    public bool m_doThing = false;
    private float m_timeBetweenSpriteChange;
    private float m_nextSpriteChange;
    private int m_spritesDisplayed;

    [SerializeField] private List<GameObject> m_spriteList;

    

    private void Awake()
    {
        //m_displayedImage = GetComponent<RawImage>(); // Gets the raw image component to swap images in and out.
        m_spritesDisplayed = m_spriteList.Count;    // Might be an off by one error here.
        //m_displayedImage = m_spriteList[m_spritesDisplayed]; // Assigns last image in list to be displayed.
        foreach(GameObject image in m_spriteList)
        {
            image.SetActive(false);
        }
        m_spriteList[m_spritesDisplayed - 1].SetActive(true); // Be aware of off by 1.
    }

    void Update()
    {
        if (m_doThing && Time.time > m_nextSpriteChange)
        {
            m_spriteList[m_spritesDisplayed - 1].SetActive(false);
            m_spritesDisplayed--;
            if (m_spritesDisplayed <= 1)
            {
                m_doThing = false;
            }
            else
            {
                m_spriteList[m_spritesDisplayed - 1].SetActive(true);
                m_nextSpriteChange = Time.time + m_timeBetweenSpriteChange;
            }
        }
    }

    public void Setup(float a_timeBetweenSpriteChange)
    {
        m_doThing = true;
        m_timeBetweenSpriteChange = a_timeBetweenSpriteChange;
        m_spriteList[m_spritesDisplayed - 1].SetActive(false);
        m_spritesDisplayed--;
        m_spriteList[m_spritesDisplayed - 1].SetActive(true);
        m_nextSpriteChange = Time.time + m_timeBetweenSpriteChange;
    }
}
