/***************************************************
 * Written By: Anton Huber
 * Purpose: Contains the problems and their qualitys
 * Data Created:14/09
 * Last Modified: 14/09
 **************************************************/
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BoxCollider))]
public class Problem : MonoBehaviour
{
    //*************************************************************************************
    // The problems themselves will contain what will fix them, the two seperate 
    // gameobjects that will will be swapped between and or an animation.
    // Should be placed on the parent Object and the active and inactive states should
    // be placed in the game world on top of each other.
    //*************************************************************************************
    
    
    
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // Be aware that if an animation needs to be played major overhaul to this class need to
    // happen. There is a way to check if an animation is complete so good chance what will
    // need to change will be the activate. Which will start the animation which then will
    // be checked everyframe until it is complete THEN the timer will be both displayed  
    // and the item will be fixable. -- I've added a pending funcion to be used when waiting 
    // for an animation to complete.
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    [HideInInspector]
    public List<Player> m_playerList; // List of players to check input and item held.
    [HideInInspector]
	public List<ParticleSystem> m_currentParticle; // Has to be turned off in deactivate.
    public List<GameObject> m_currentProjectile;
    [SerializeField] private ProblemController m_uiPrefab;
    [SerializeField] private Vector3 m_uiPosition = new Vector3(0,4,0); // Ui can be set elsewhere.
    private ProblemController m_ui;
    [SerializeField] private GameObject m_inactiveState; // Place the problems inactive state model here.
   
    [SerializeField] private List<ProblemList> m_problemList; // A list of all the problems that can take place on this object. // Need this to extend out.
    private ProblemList m_currentProblem; // All the info about the currently selected problem.

    
    private bool m_isPending = false; 
    private bool m_isActive = false; // Is the problem active or inactive. Can be used to trigger other events like ui or particle effects.
    private float m_timeBeforeFail; // Once this value is reached the problem will be expired and health will be lost.

    private int m_buttonPresses; // The counter for button presses.

    //[SerializeField] private List<SoundRequester> m_sounds;            
    
 

    void Start()
    {
        m_currentParticle = new List<ParticleSystem>();
        m_ui = Instantiate<ProblemController>(m_uiPrefab);
        m_ui.Setup(this, m_uiPosition);
        m_inactiveState.SetActive(true);
        foreach (ProblemList problem in m_problemList)
        {
            problem.GetActiveState().SetActive(false);
            if (problem.GetSounds() != null)
                if (problem.GetSounds().Count != 0)
                {
                    foreach (SoundRequester sounds in problem.GetSounds())
                    {
                        sounds.SetupSound(this.gameObject);
                    }
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ////TEST
        //if(m_currentProblem != null)
        //if (m_currentProblem.GetSounds() != null)
        //    if (m_currentProblem.GetSounds().Count != 0)
        //        if (Input.anyKeyDown)
        //{
        //     Timer.SoundMangerGet().Play(m_currentProblem.GetSounds(), WhenToPlaySound.Walk);
        //}
        ////TEST

        // Check if the object is pending.
        
        if (m_playerList != null)
            if (m_playerList.Count > 0 && m_isActive)
            {
                Actions();
            }
        SoundManager s = Timer.SoundMangerGet();
        // Sounds for problem would go here.
        if(m_currentProblem != null)
            s.CheckAudio(m_currentProblem.GetSounds()); // The problems for the specified problem occuring
    }


    public float GetTime()
    {
        // Returns the time the problem will expire on.
        return m_timeBeforeFail; // In case it needs to be called outside of class. Mostly for 'ProblemLists' implementation.
    }


    public bool GetActive()
    {
        // Returns if the problem is active or not.
        return m_isActive;
    }

    public bool GetPending()
    {
         return m_isPending;
    }

    public int GetButtonPressCount()
    {
        return m_buttonPresses;
    }

    public int GetButtonPressesMax()
    {
        return m_currentProblem.GetMaxButtonPresses();
    }


    public void Pending()
    {
        m_isPending = true;
        m_isActive = true; // Activate the object
        m_currentProblem = m_problemList[Random.Range(0, m_problemList.Count)]; // Assaigns a problem on the list of problems to be the current problem occuring.
        if (m_currentProblem.GetBreakParticleEffect() != null) // Checks if there is a particle effect in here for the associated problem and then activates then returns.
            if (m_currentProblem.GetBreakParticleEffect().Count != 0)
            {
                List<GameObject> particle = m_currentProblem.GetBreakParticleEffect();
                for (int i = 0; i < m_currentProblem.GetBreakParticleEffect().Count; i++)
                {
                    GameObject t = Instantiate<GameObject>(particle[i]);
                    //m_currentParticle.Add(t);
                    m_currentProjectile.Add(t);
                    m_currentProjectile[i].transform.position = this.transform.position + new Vector3(0, m_currentProblem.GetParticleStartDistance(), 0); // Sets the particle effect to start off sceen above the problem.
                    //m_currentParticle[i].Play();
                }
            }
        if(m_currentProblem.GetSounds() != null)
            if(m_currentProblem.GetSounds().Count != 0)
                    Timer.SoundMangerGet().Play(m_currentProblem.GetSounds(), WhenToPlaySound.Pending);
    }

    public bool CheckPending()
    {
        // Will need a complete revamp due to animator and coutines.


        if (m_currentProblem.GetBreakParticleEffect() != null)
        {
            if (m_currentProblem.GetBreakParticleEffect().Count != 0)
                //if (m_currentParticle[0].isPlaying) // Specific case for particle effect as upon completion 
                {
                    Vector3 move = new Vector3(0, m_currentProblem.GetParticleFallSpeed() * Time.deltaTime, 0);                  // Only should be movement on y axis
                    foreach (GameObject projectile in m_currentProjectile)                               // m_curentParicle.transform.position = move.normalized;
                    {
                        projectile.transform.position -= move; // Will move each particle. -- Old single direction sysetm
                    }
                    if (m_currentProjectile[0].transform.position.y <= this.transform.position.y) // Check if it has moved past the object.
                    {
                        foreach (GameObject projectile in m_currentProjectile)
                        {
                            //projectile.Stop();
                            Destroy(projectile.gameObject, 2);
                        }
                        m_currentProjectile.Clear();
                        m_isPending = false;
                        return true; // May cause issues with 
                                     // Animator needs to check for a 'Start' animation. if it does play it. If it doesn't skip.
                    }
                    return false;
                }
        }
        // Animator needs to check for a 'Start' animation. if it does play it. If it doesn't skip.
        m_isPending = false;
        return true;
    }

    public void Activate(float a_timerBeforeFail)
    {
        m_isPending = false;
        m_currentProblem.GetActiveState().SetActive(true); // Displays the problems active Mesh.
        if (m_currentProblem.GetPhsyicsObject() != null)
        {
            m_currentProblem.GetPhsyicsObject().ApplyForce(); // Applies the force.
        }
        if (m_currentProblem.GetBreakParticleEffect() != null)
            if (m_currentProblem.GetBreakParticleEffect().Count != 0)
            {
                foreach (ParticleSystem particle in m_currentParticle)
                {
                    particle.Stop();
                    Destroy(particle.gameObject, 2);
                }
                m_currentParticle.Clear();
            }
        if (m_currentProblem.GetWhileBrokenParticle() != null)
            if (m_currentProblem.GetWhileBrokenParticle().Count != 0)
            {
                List<ParticleSystem> particles = m_currentProblem.GetWhileBrokenParticle();
                for (int i = 0; i < m_currentProblem.GetWhileBrokenParticle().Count; i++)
                {
                    ParticleSystem t = Instantiate<ParticleSystem>(particles[i]);
                    t.Play();
                    t.transform.position = this.transform.position;
                    m_currentParticle.Add(t);
                }
            }

        //SoundManager.PlaySound(m_sounds, Sound.Active);


        m_buttonPresses = 0;// This sets m_buttonpresses to 0.
        m_inactiveState.SetActive(false); // Hides the problems inactive Mesh.
        m_timeBeforeFail = a_timerBeforeFail;// Assaigns it a time frame before a life is lost.
        m_ui.Activate();

    }

    public void Deactivate()
    {
        if (m_currentProblem.GetPhsyicsObject() != null)
        {
            m_currentProblem.GetPhsyicsObject().ResetToStartPosition();
        }
        if (m_currentProblem.GetWhileBrokenParticle() != null)
            if (m_currentProblem.GetWhileBrokenParticle().Count != 0)
            {
                foreach (ParticleSystem particle in m_currentParticle)
                {
                    particle.Stop();
                    Destroy(particle.gameObject, 2);
                }
                m_currentParticle.Clear();
            }

        // Success case
        if (m_buttonPresses >= m_currentProblem.GetMaxButtonPresses())
        {
            if (m_currentProblem.GetFixedParticle() != null) // Checks if any particle plays.
                if (m_currentProblem.GetFixedParticle().Count != 0)
                {
                    List<ParticleSystem> particles = m_currentProblem.GetFixedParticle();
                    foreach (ParticleSystem particle in particles)
                    {
                        ParticleSystem t = Instantiate<ParticleSystem>(particle);
                        t.Play();
                        t.transform.position = this.transform.position;
                        // Could use a stop here.
                        Destroy(t.gameObject, 2);
                    }
                    m_currentParticle.Clear();
                }
        }
        // Fail case
        else if (m_currentProblem.GetFailedParticle() != null) // Checks if any particle plays.
            if (m_currentProblem.GetFailedParticle().Count != 0)
            {
                List<ParticleSystem> particles = m_currentProblem.GetFailedParticle();
                foreach (ParticleSystem particle in particles)
                {

                    ParticleSystem t = Instantiate<ParticleSystem>(particle);
                    t.Play();
                    t.transform.position = this.transform.position;
                    // Could use a stop here.
                    Destroy(t.gameObject, 2);
                }
                m_currentParticle.Clear();
            }
            m_ui.gameObject.SetActive(false);
            m_ui.ResetSprites();
            m_isActive = false;
        if (m_currentProblem.GetAnimationController() == null)
        {
            ModelSwap();
        }
        else
        {
            m_currentProblem.GetAnimationController().SetTrigger("EndAnimation");
        }
        m_currentProblem = null;
    }

    public void ModelSwap()
    {
        m_inactiveState.SetActive(true); // Re-enables the inactive game object
        m_currentProblem.GetActiveState().SetActive(false); // Hides the problem active Mesh. -- May need review.
    }


    public bool CheckActive(float a_timeLimit)
    {
        //@Checks the state of currently activated objects. If expired return true.
        return (m_timeBeforeFail >= a_timeLimit && m_isActive);
    }

	private void Actions()
	{
		for (int i = 0; i < m_playerList.Count; i++)
		{
			Player player = m_playerList[i];
            if (player.GetItem() != null)
            {
                Item item = player.GetItem();
                //XboxController controller = m_playerList[i].controller;
                if(Timer.GetKeyBoard())
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (CompareQuality(item))
                        {
                            if (item.IsRefillable())
                            {
                                if (item.GetCharges() > 0)
                                {
                                    m_buttonPresses++; // this has to be 

                                }
                                else
                                {
                                    // Can play an animation on the player to indicate that it's empty.
                                }
                            }
                            else
                            {
                                m_buttonPresses++; // This needs to increment.
                            }
                            // Get forward vector of player dot product against postion of problem? 
                            // Check if it is within the active 
                            // if player is then rotate directly towards the problem and play swing animation.
                            // Applies - to button presses.
                            if (m_buttonPresses >= this.GetButtonPressesMax())
                            {
                                if (item.IsRefillable())
                                {
                                    item.UseCharge();
                                    //Does math to see what plane to display.
                                }
                                else if (item.IsOneTimeUse())
                                {
                                    item.GetSpawner().m_spawnedItemList.Remove(item);
                                    player.DropItem();                                     // Detaches the item from the palyer and vice versa
                                    item.enabled = false;
                                    Destroy(item.gameObject);                              // Destroys the item.
                                }
                                Deactivate();
                                Timer.SolvedProblem();
                            }
                        }
                    }
                }
                else
                {
                    if (XCI.GetButtonDown(XboxButton.A, player.controller))
                    {
                        if (CompareQuality(item))
                        {
                            if (item.IsRefillable())
                            {
                                if (item.GetCharges() > 0)
                                {
                                    m_buttonPresses++; // this has to be 

                                }
                                else
                                {
                                    // Can play an animation on the player to indicate that it's empty.
                                }
                            }
                            else
                            {
                                m_buttonPresses++; // This needs to increment.
                            }
                            // Get forward vector of player dot product against postion of problem? 
                            // Check if it is within the active 
                            // if player is then rotate directly towards the problem and play swing animation.
                            // Applies - to button presses.
                            if (m_buttonPresses >= this.GetButtonPressesMax())
                            {
                                if (item.IsRefillable())
                                {
                                    item.UseCharge();
                                    //Does math to see what plane to display.
                                }
                                else if (item.IsOneTimeUse())
                                {
                                    item.GetSpawner().m_spawnedItemList.Remove(item);
                                    player.DropItem();                                     // Detaches the item from the palyer and vice versa
                                    item.enabled = false;
                                    Destroy(item.gameObject);                              // Destroys the item.
                                }
                                Deactivate();
                                Timer.SolvedProblem();
                            }
                        }
                    }
                }
            }
		}
	}

	// Collision experiment
	private void OnTriggerEnter(Collider other)
	{
		// Insert players to the list 
		int count = m_playerList.Count;
		if (other.tag == "Player")
		{
			Player holder = other.GetComponent<Player>();

			m_playerList.Insert(count, Timer.PlayerGet(holder.GetPlayerIndex()));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		// Remove player from the list
		if (other.tag == "Player")
		{
			Player holder = other.GetComponent<Player>();

			m_playerList.Remove(Timer.PlayerGet(holder.GetPlayerIndex()));
		}
	}


	private bool CompareQuality(Item a_item)
    {   
        // Used to compare the item the player currently has against the problems that are currently wrong with it.
        // Currently compares all the qualites that the problem can have. Needs to be changed to the actuve one.
            foreach (E_Quality itemquality in a_item.m_fixableQuality)
                if (m_currentProblem.GetQuality() == itemquality)
                    return true;
                else
                    return false;
        return false; // If you hit this then there is a problem.
    }

    //private void PlaySound(WhenToPlaySound a_lookingForSound)
    //{
    //    List<SoundRequester> soundbank = m_currentProblem.GetSounds();
    //    if (soundbank != null)
    //    {
    //        if(soundbank.Count != 0)
    //        {
    //            foreach ()
    //        }
    //    }
    //}
}