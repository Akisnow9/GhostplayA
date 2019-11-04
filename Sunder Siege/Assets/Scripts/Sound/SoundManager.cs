using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


// This will be a variable attached to the timer class.
// Everything can call the timer.
// OR
// It can exist seperatly in the menu to allow music and sfx.
[System.Serializable]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<AudioMixerGroup> m_mixers;

    [SerializeField] private List<Sound> m_soundBank;
    // On start for each problem it needs to request and save a copy of it's auido source if it requires a loop.

    //void Awake()
    //{
    //    foreach (Sound s in m_soundBank)
    //    {
    //        if(s.m_Identifier == SoundToPlay.None)
    //        {
    //            Debug.Log("No sound identifier attached to " + s.m_name + " so it can never be requested.");
    //            Debug.Log("Check the SoundManager.");
    //        }
    //        // All data contained within sound needs to now be applied to
    //        // the audio source created.
    //        s.m_source = gameObject.AddComponent<AudioSource>();
    //        s.m_source.clip = s.m_soundClip;
    //        s.m_source.loop = false;
    //        s.m_source.pitch = s.m_pitch;
    //        s.m_source.volume = s.m_volume;
    //        s.m_source.outputAudioMixerGroup = GetMixer(s);
    //    }
    //}

    private void Awake()
    {
        foreach (Sound s in m_soundBank)
        {
            if (s.m_Identifier == SoundToPlay.None)
            {
                Debug.Log("No sound identifier attached to " + s.m_name + " so it can never be requested.");
                Debug.Log("Check the SoundManager.");
            }
        }
    }

    private AudioMixerGroup GetMixer(Sound a_mixer)
    {
        switch (a_mixer.m_mixer)
        {
            case Mixer.DiageticMusic:
                {
                    return m_mixers[1]; // Diagetic Music
                }
            case Mixer.NonDiageticMusic:
                {
                    return m_mixers[2]; // NonDiagetic Music
                }
            case Mixer.DiageticSfx:
                {
                    return m_mixers[3]; // Diagetic Sfx
                }
            case Mixer.NonDiageticSfx:
                {
                    return m_mixers[4]; // NonDiagetic Sfx
                }
            default:
                {
                    {
                        Debug.Log(a_mixer.m_name + " not assaigned a mixer.");
                        return null;
                    }
                }
        }

    }

    //public Sound GetSound(SoundToPlay a_identifier)
    //{
    //    switch(a_identifier)
    //    {
    //        case SoundToPlay.Walking:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Running:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Throw:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Bounce:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Burning:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Fix:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Fail:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Crumble:
    //            {
    //                return null;
    //            }
    //        case SoundToPlay.Projectile:
    //            {
    //                return null;
    //            }
    //        default:
    //            {
    //                Debug.Log("Sound " + a_identifier + " not found.");
    //                return null;
    //            }
    //    }
    //}

    //public Sound GetSound(SoundToPlay a_requestedSound) // Changed return from 'sound' to 'audiosource'.
    //{
    //    foreach (Sound s in m_soundBank)
    //    {
    //        if(a_requestedSound == s.m_Identifier)
    //        {
    //            return s;
    //        }
    //    }
    //    Debug.Log("Sound " + a_requestedSound.ToString() + "Has not been asaigned in soundbank");
    //    return null;
    //}


    public Sound GetSound(GameObject a_parent, SoundToPlay a_requestedSound)
    {
        foreach (Sound s in m_soundBank)
        {
            if (a_requestedSound == s.m_Identifier)
            {
                // Most of this is safety precations. It does double up some things.
                s.m_source = a_parent.AddComponent<AudioSource>();
                s.m_source.clip = s.m_soundClip;
                s.m_source.loop = false;
                s.m_source.pitch = s.m_pitch;
                s.m_source.volume = s.m_volume;
                s.m_source.outputAudioMixerGroup = GetMixer(s);
                s.m_source.playOnAwake = false;
                return s;
            }
        }
        Debug.Log("Sound " + a_requestedSound.ToString() + "Has not been asaigned in soundbank");
        return null;

    }

    public void Play(List<SoundRequester> a_soundbank, WhenToPlaySound a_LookingForSound)
    {
        foreach (SoundRequester sound in a_soundbank)
        {
            if (a_LookingForSound == sound.m_trigger)
            {
                sound.m_isActive = true;
                sound.Play();
                return;
            }
        }
    }

    // Needs to be called by everything  during their update as no centralised place for audio
    // 
    public void CheckAudio(List<SoundRequester> a_checkSounds)
    {
        foreach (SoundRequester soundgroup in a_checkSounds)
        {
            if (soundgroup.m_isActive)
            {
                if (soundgroup.m_fadeOut && soundgroup.m_fadeOutStart < Time.time) // fade out
                {
                    soundgroup.FadeOut();
                }
                else if (soundgroup.m_sequence && soundgroup.m_loop != true)
                {
                    soundgroup.NextInSequence(); // Checks if next sound to be played.
                }
                else if (soundgroup.m_fadeIn && soundgroup.m_soundBank[0].m_volume != soundgroup.m_maxVolume) // fade in
                {
                    soundgroup.FadeIn();
                }
            }
            soundgroup.CheckIfPlaying();
        }
    }
}