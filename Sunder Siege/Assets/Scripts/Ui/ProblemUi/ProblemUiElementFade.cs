using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemUiElementFade : MonoBehaviour
{
    private float m_fadeSpeed; // The amount of opacity applied to the gameobject. -- Should be a negative or positive
    private SpriteRenderer m_colourFader; // The colour of the sprite.


    private void Start()
    {
        m_colourFader = this.GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if(m_fadeSpeed > 0) // Fade in
        {
           // m_colourFader.color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, m_fadeSpeed);
        }
        else if(m_fadeSpeed < 0) // Fade out
        {
            //m_colourFader.color = new Color(1,1,1,Mathf.SmoothStep(0, 1, m_fadeSpeed);
        }
    }

    public void FadeOut(float a_fadeSpeed)
    {
        this.gameObject.SetActive(true);
        m_fadeSpeed = a_fadeSpeed;
    }
    public void FadeIn(float a_fadeSpeed)
    {
        this.gameObject.SetActive(true);
        m_colourFader.color = new Color(1, 1, 1, 0);
        m_fadeSpeed = a_fadeSpeed;
    }

}
