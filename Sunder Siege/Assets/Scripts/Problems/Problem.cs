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
    public List<Player> m_playerList; // List of players to check input and item held.
	public ParticleSystem m_currentParticle; // Has to be turned off in deactivate.

    [SerializeField] private ProblemController m_uiPrefab;
    private ProblemController m_ui;
    [SerializeField] private GameObject m_inactiveState; // Place the problems inactive state model here.
   
    [SerializeField] private List<ProblemList> m_problemList; // A list of all the problems that can take place on this object. // Need this to extend out.
    private ProblemList m_currentProblem; // All the info about the currently selected problem.

    
    private bool m_isPending = false; 
    private bool m_isActive = false; // Is the problem active or inactive. Can be used to trigger other events like ui or particle effects.
    private float m_timeBeforeFail; // Once this value is reached the problem will be expired and health will be lost.

    private int m_buttonPresses; // The counter for button presses.


   
   



    // [SerializeField] private ProblemUi m_ui; Might need to be canvas.

    // private ParticleSystem m_currentParticle; // Has to be turned off in deactivate.
    // Start is called before the first frame update
    void Start()
    {

        m_ui = Instantiate<ProblemController>(m_uiPrefab);
        m_ui.Setup(this);
        m_inactiveState.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the object is pending.
        
        if (m_playerList != null)
            if (m_playerList.Count > 0 && m_isActive)
            {
                Actions();
            }
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
        {
            ParticleSystem t = Instantiate<ParticleSystem>(m_currentProblem.GetBreakParticleEffect());
            m_currentParticle = t;
            m_currentParticle.transform.position = this.transform.position + new Vector3(0, m_currentProblem.GetParticleStartDistance(), 0); // Sets the particle effect to start off sceen above the problem.
            m_currentParticle.Play();
        }
        // Animator needs to check for a 'Start' animation. if it does play it. If it doesn't skip.
	}


    public void Activate(float a_timerBeforeFail)
    {
        m_isPending = false;
        m_currentProblem.GetActiveState().SetActive(true); // Displays the problems active Mesh.
        if (m_currentProblem.GetPhsyicsObject() != null)
        {
            //m_breakPhysics[m_currentProblemIterator].enabled = true;
            m_currentProblem.GetPhsyicsObject().ApplyForce(); // Applies the force.
        }
        if (m_currentProblem.GetWhileBrokenParticle() != null)
			{
                ParticleSystem t = Instantiate<ParticleSystem>(m_currentProblem.GetWhileBrokenParticle());
                m_currentParticle = t;
                m_currentParticle.transform.position = this.transform.position;
                m_currentParticle.Play();
			}
        m_buttonPresses = 0;// This sets m_buttonpresses to 0.
        m_inactiveState.SetActive(false); // Hides the problems inactive Mesh.
        m_timeBeforeFail = a_timerBeforeFail;// Assaigns it a time frame before a life is lost.
        m_ui.Activate();
    }

    public void Deactivate()
    {
        if(m_currentProblem.GetWhileBrokenParticle() != null)
        {
            m_currentParticle.Stop();
            Destroy(m_currentParticle.gameObject, 2);
        }
        m_isActive = false;
        m_inactiveState.SetActive(true); // Re-enables the inactive game object
        m_currentProblem.GetActiveState().SetActive(false); // Hides the problem active Mesh.
        // Add itself to the top of the list. - Will need to be done from m_problemList
        // Removes itself from current postion in list. - Will need to be done from m_problemList
        if(m_currentProblem.GetFixedParticle() != null)
        {
            ParticleSystem t = Instantiate<ParticleSystem>(m_currentProblem.GetFixedParticle());
            m_currentParticle = t;
            m_currentParticle.transform.position = this.transform.position;
            m_currentParticle.Play(); // Particle effect needs to be prewarmed and shouldn't exist for too long.
            m_currentParticle.Stop(); 
            Destroy(m_currentParticle.gameObject, 2);
        }
        if(m_currentProblem.GetPhsyicsObject() != null)
        {
            m_currentProblem.GetPhsyicsObject().ResetToStartPosition();
        }
        m_ui.ResetSprites();
        m_ui.gameObject.SetActive(false);
    }

	public bool CheckPending()
	{
        // Will need a complete revamp due to animator and coutines.


            if (m_currentProblem.GetBreakParticleEffect() != null)
			{
				if (m_currentParticle.isPlaying) // Specific case for particle effect as upon completion 
				{
					Vector3 move = new Vector3(0, m_currentProblem.GetParticleFallSpeed() * Time.deltaTime, 0); // Only should be movement on y axis
                                                                                            // m_curentParicle.transform.position = move.normalized;
                    m_currentParticle.transform.position -= move;

                if (m_currentParticle.transform.position.y < this.transform.position.y) // Check if it has moved past the object.
                {
                    m_currentParticle.Stop();
                    Destroy(m_currentParticle.gameObject, 2); 
                    m_isPending = false;
                    return true;
                    // Animator needs to check for a 'Start' animation. if it does play it. If it doesn't skip.
                }
                //else
                //{
                //    m_isPending = false;
                //    return true;

                //}
                return false;
				}
			}
        // Animator needs to check for a 'Start' animation. if it does play it. If it doesn't skip.
        m_isPending = false;
	    return true;
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
}