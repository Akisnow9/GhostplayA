using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// Will mostly contain functions for the sounds that are requeted as well as options
// for sound layering.
// Will need to add lots of safety checks and options for how the sounds should be played
// which will be handled by the class itself using the varibales set in editor.

[System.Serializable]
public class SoundRequester
{
    [SerializeField] private string m_name;
    [SerializeField] public WhenToPlaySound m_trigger;
    [SerializeField] private List<SoundToPlay> m_requestedSounds;

    [HideInInspector]
    public List<Sound> m_soundBank; // Might be able to change to audio source.

    [Header("Do you want a delay between the sounds?")]
    [SerializeField] private bool m_enableDelay;
    [Range(0.1f, 2.0f)]
    [SerializeField] private float m_delay = 0.1f;
    [Header("The clip will only be played for")]
    [Range(0.0f, 3.0f)]
    [SerializeField] private float m_clipLength = 0.5f;

    [Header("Does the sound loop? Only really useful for ambient sounds(burning) and music.")]
    public bool m_loop = false;

    [Header("Will the sound need to increase over time?")]
    public bool m_fadeIn = false;
    [Header("If it does, what level does it start at?")]
    [Range(0f, 1.0f)]
    [SerializeField] private float m_startVolume = 1.0f;
    [Header("What level does it increase to?")]
    [Range(0.1f, 1.0f)]
    public float m_maxVolume = 1.0f;

    [Header("How quickly does it increase?")]
    [Range(0.001f, 0.01f)]
    [SerializeField] private float m_increaseRate = 0.01f;
    
    [Header("Will the sound need to play all requested sounds for the " +
        "trigger all at once or one after each other?")]
    public bool m_sequence = false;

    [Header("Will the sound need to decrease over time?")]
    public bool m_fadeOut = false;
    [Header("If it does, what level does it stop playing at?")]
    [Range(0f, 1.0f)]
    [SerializeField] private float m_minVolume = 0.2f;
    [Header("How quickly does it reduce?")]
    [Range(0.001f, 0.01f)]
    [SerializeField] private float m_reductionRate = 0.01f;
    [Header("How long till it starts?")]
    [Range(0, 60)]
    public int m_timeTillFade = 5;


    [Header("How 3D is the sound? Advised to use something between .2 and .8.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_spatialBlend = 0.7f;



    // These are all variables for the class to keep track of what it is doing.
    [HideInInspector]
    public float m_lastPlayed;
    [HideInInspector]
    public bool m_isActive; // Will skip over soundrequesters that are inactive
    private int m_playingSound = 0; // Used for 'sequence' so data knows where it is.
    [HideInInspector]
    public float m_fadeOutStart;
    [HideInInspector]
    public float m_delayOffset;

    public void SetupSound(GameObject a_parent)
    {
        m_soundBank = new List<Sound>();
        SoundManager soundmanager = Timer.SoundMangerGet();
        m_playingSound = 0;
        foreach (SoundToPlay sound in m_requestedSounds)
        {
            m_soundBank.Insert(m_playingSound, soundmanager.GetSound(a_parent, sound));
            // All data above gets copied here.
            AudioSource s = m_soundBank[m_playingSound].m_source;
            s.loop = m_loop;
            s.spatialBlend = m_spatialBlend;
            m_playingSound++;
        }
    }


    // Called outside once correct sound is found.
    // As in the loop needs to exis somewhere else.
    // OR
    // can pass through the whole list and have that stored
    public void Play()
    {

        if(m_fadeOut)
        {
            m_fadeOutStart = Time.time + (float)m_timeTillFade;
        }
        if (m_sequence)
        {
            if (!m_soundBank[m_playingSound].m_source.isPlaying)
            {
                m_playingSound++;
                if(m_fadeIn)
                {
                    m_soundBank[m_playingSound].m_source.volume = m_startVolume;
                }
                m_soundBank[m_playingSound].m_source.Play();
                this.m_isActive = true;
            }
        }
        else
        {
            // Mostly used for short sounds.
            if (m_enableDelay)
            {
                if (m_lastPlayed < Time.time)
                {
                    foreach (Sound s in m_soundBank)
                    {
                        if (s.m_source.pitch > 1.5f || s.m_source.pitch < 0.3f)
                        {
                            s.m_source.volume = s.m_volume;
                            s.m_source.pitch = s.m_pitch;
                        }
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) < 0.5f)
                            {
                                s.m_source.pitch = s.m_source.pitch + Random.Range(0.1f, 0.3f);
                            }
                            else
                            {
                                s.m_source.pitch = s.m_source.pitch - Random.Range(0.1f, 0.3f);
                            }
                        }
                        s.m_source.Play();
                        m_lastPlayed = Time.time + m_delay;
                        m_delayOffset = Time.time + m_clipLength;
                    }
                }
            }
            else
            {
                foreach (Sound s in m_soundBank)
                {
                    if(m_fadeIn)
                    {
                        s.m_source.volume = m_startVolume;
                    }
                    s.m_source.Play();
                }
                    m_isActive = true;
            }
        }
    }


    // Might need functions for hard knees
    //
    public void Reset()
    {
        foreach(Sound s in m_soundBank)
        s.m_source.volume = s.m_volume;
    }


    public void FadeIn()
    {
      {
            foreach (Sound s in m_soundBank)
            {
                s.m_source.volume = s.m_source.volume + m_increaseRate; // This is wrong but eh
                if (s.m_source.volume > m_maxVolume * s.m_volume)
                {
                    s.m_source.volume = m_maxVolume * s.m_volume;
                }
            }
      }
    }

    public void FadeOut()
    {
        if (m_sequence)
        {
            m_soundBank[m_playingSound].m_source.volume = m_soundBank[m_playingSound].m_source.volume - this.m_reductionRate;
            if (m_soundBank[m_playingSound].m_source.volume < m_minVolume)
            {
                m_soundBank[m_playingSound].m_source.Stop();
            }
        }
        else
        {
            foreach (Sound s in m_soundBank)
            {
                s.m_source.volume = s.m_source.volume - this.m_reductionRate; // This is wrong but eh
                if (s.m_source.volume < m_minVolume)
                {
                    s.m_source.Stop();
                }
            }
        }
    }

    public void NextInSequence()
    {
        if (!m_soundBank[m_playingSound].m_source.isPlaying)
        {
            if (m_soundBank.Count <= m_playingSound - 1) // Maybe - 1
            {
                if(m_fadeOut)
                {
                    m_fadeOutStart = Time.time + (float)m_timeTillFade;
                }
                m_playingSound++;
                if (m_fadeIn)
                {
                    m_soundBank[m_playingSound].m_source.volume = this.m_startVolume;
                }
                m_soundBank[m_playingSound].m_source.Play();
            }
            else
            m_isActive = false;
        }
    }

    public void CheckIfPlaying()
    {
        foreach(Sound s in m_soundBank)
        {
            if(m_enableDelay)
            {
                s.m_source.volume -= 0.01f;
                if (m_delayOffset < Time.time)
                {
                    s.m_source.Stop();
                    s.m_source.pitch = s.m_pitch;
                    s.m_source.volume = s.m_volume;
                }
            }
            if (s.m_source.isPlaying)
            {
                return; // As long as sounds are playing it will hit this.
            }
        }
        // Once all sounds have stopped it will hit this.
        m_isActive = false;
    }

}