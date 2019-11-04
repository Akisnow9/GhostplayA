
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string m_name;
    public SoundToPlay m_Identifier;
    public AudioClip m_soundClip;


    //[Range(-0.05f, 3.0f)]
    //public float m_pitchRangeMin;
    [Range(-0.05f, 3.0f)]
    public float m_pitch = 1.0f;
    [Range(0f, 1f)]
    public float m_volume = 1.0f;
    public Mixer m_mixer = Mixer.None;

    [HideInInspector]
    public AudioSource m_source;
    //private bool m_diagetic; // Might not need.


    // Consturctor for copying sounds.
    // I only really need audiosources since the data is applied in 
    // the awake step of soundmanager already...
    // I suppose it does allow me to move the 'awake' to the indivdual
    // to the requester to do.
    // I will revisit this.
    public Sound(Sound a_soundCopy, GameObject a_parent)
    {

        this.m_source = a_soundCopy.m_source;
        this.m_name = a_soundCopy.m_name;
        this.m_Identifier = a_soundCopy.m_Identifier;
        this.m_soundClip = a_soundCopy.m_soundClip;
        this.m_pitch = a_soundCopy.m_pitch;
        this.m_volume = a_soundCopy.m_volume;
        this.m_mixer = a_soundCopy.m_mixer;
    }

}
