using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    // Just a class that deals with the sound of a projectile.
    // Could instead be a single class handling sound.

    [SerializeField]
    private List<SoundRequester> m_sounds;
    // Start is called before the first frame update
    void Start()
    {
        if (m_sounds != null)
            if (m_sounds.Count != 0)
            {
                foreach (SoundRequester sounds in m_sounds)
                {
                    sounds.SetupSound(this.gameObject);
                }
            }
        Timer.SoundMangerGet().Play(m_sounds, WhenToPlaySound.Fall);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_sounds.Count != 0)
            Timer.SoundMangerGet().CheckAudio(m_sounds);
    }
}
