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
	public List<ParticleSystem> m_currentParticle; // Has to be turned off in deactivate.

    [SerializeField] private GameObject m_inactiveState; // Place the problems inactive state model here.
    [SerializeField] private GameObject m_activeState; // Place the problems active state model here.

    private MeshRenderer m_inactiveMeshRenderer; // Gets the MeshRenderer in startup to turn on/off inactive model.
    private MeshRenderer m_activeMeshRenderer; // Gets MeshRendere in startup to turn on/off active model.


    [SerializeField] private List<E_Quality> m_fixableProblems; // Will be able to set the list size and the problems from the unity editor.
	[SerializeField] private E_Quality m_currentProblem; // The problem chosen from the above. Will call particle effects and animation. eg fixable problem 0 on list callse particle effect 0(if it exitss) completes then animation 0(if it exists) then acitvates.
    private int m_currentProblemIterator; // A number that keeps track of what breakparticleeffect/breakanimation/whilebrokenparticleeffect needs to trigger.


    private bool m_isPending = false; 
    private bool m_isActive = false; // Is the problem active or inactive. Can be used to trigger other events like ui or particle effects.
    private float m_timeBeforeFail; // Once this value is reached the problem will be expired and health will be lost.

    [SerializeField] private List<int> m_maxButtonPresses; // How many wacks it takes to fix the problem. Assaigned every activation. Per event.
    private int m_buttonPresses; // The counter for button presses.


    //*************************************************************************************
    // Extra variables should go here for animation or turning on and off particle effects.
    //*************************************************************************************

    [SerializeField] private List<ParticleSystem> m_breakParticleEffect; // What particle effects take place. 1 to 1 ration with animation and Fixable problem.  // Could probably be changed to game object that is turned on and off.
    [SerializeField] private float m_particleStartDistance = 3; // Where the particel will spawn
    [SerializeField] private float m_particleFallSpeed = 0.5f; // Controls the speed of the particle falling.
    [SerializeField] private List<Animation> m_breakAnimation; // What happens when it breaks? 1 to 1 ratio with paritcle and fixable problems.
    [SerializeField] private List<ParticleSystem> m_whileBrokenParticle; // Will just loop while the problem is active. 1 to 1.


    // private ParticleSystem m_currentParticle; // Has to be turned off in deactivate.
    // Start is called before the first frame update
    void Start()
    {
        m_inactiveMeshRenderer = m_inactiveState.GetComponent<MeshRenderer>(); // Gets the meshrenderer to turn off 
        m_inactiveMeshRenderer.enabled = true; // Displays correct state.
        m_activeMeshRenderer = m_activeState.GetComponent<MeshRenderer>(); // Gets the meshrenderer to turn off 
        m_activeMeshRenderer.enabled = false; // Displays correct state.
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

public void Pending()
	{
        m_isPending = true;
		m_isActive = true; // Activate the object
		m_currentProblemIterator = Random.Range(0, m_fixableProblems.Count); // Need this int so that we can trigger appropriate animation or paricle effect. // is it an off by 1 error?
		m_currentProblem = m_fixableProblems[m_currentProblemIterator]; // Sets the quality enum.
		if (m_breakParticleEffect.Count != 0)
			if (m_breakParticleEffect[m_currentProblemIterator] != null) // Checks if there is a particle effect in here for the associated problem and then activates then returns.
			{
                ParticleSystem t = Instantiate<ParticleSystem>(m_breakParticleEffect[m_currentProblemIterator]);
                m_currentParticle.Insert(0, t);
                m_currentParticle[0].transform.position = this.transform.position + new Vector3(0, m_particleStartDistance); // Sets the particle effect to start off sceen above the problem.
                m_currentParticle[0].Play();
			}
			else if (m_breakAnimation.Count != 0)
				if (m_breakParticleEffect[m_currentProblemIterator] != null)
				{
					m_breakAnimation[m_currentProblemIterator].Play(); // Plays animation
				}
	}


    public void Activate(float a_timerBeforeFail)
    {
        m_isPending = false;
		if (m_whileBrokenParticle.Count != 0)
			if (m_whileBrokenParticle[m_currentProblemIterator] != null)
			{
                ParticleSystem t = Instantiate<ParticleSystem>(m_whileBrokenParticle[m_currentProblemIterator]);
                m_currentParticle.Insert(0, t);
                m_currentParticle[0].transform.position = this.transform.position;
                m_currentParticle[0].Play();
			}
        m_buttonPresses = m_maxButtonPresses[m_currentProblemIterator];
        m_inactiveMeshRenderer.enabled = false; // Hides the problems inactive Mesh.
        m_activeMeshRenderer.enabled = true; // Displays the problems active Mesh.
        m_timeBeforeFail = a_timerBeforeFail;// Assaigns it a time frame before a life is lost.
        //Add itself to the bottom of the list. - Will need to be done from m_problemList
        //Remove itself from current position in list. - Will need to be done from m_problemList
    }

    public void Deactivate()
    {
        if (m_whileBrokenParticle.Count != 0)
            if (m_whileBrokenParticle[m_currentProblemIterator] != null) // THis is the problem.
            {
                m_currentParticle[0].Stop();
                ParticleSystem t = m_currentParticle[0];
                m_currentParticle.Remove(t);
                Destroy(t.gameObject, 2);
            }
        m_isActive = false;
        m_inactiveMeshRenderer.enabled = true;// Displays the problem inactive Mesh.
        m_activeMeshRenderer.enabled = false; // Hides the problem active Mesh.
        // Add itself to the top of the list. - Will need to be done from m_problemList
        // Removes itself from current postion in list. - Will need to be done from m_problemList
    }

	public bool CheckPending()
	{

		if (m_breakParticleEffect.Count != 0)
			if (m_breakParticleEffect[m_currentProblemIterator] != null)
			{
				if (m_currentParticle[0].isPlaying) // Specific case for particle effect as upon completion 
				{
					Vector3 move = new Vector3(0, m_particleFallSpeed * Time.deltaTime, 0); // Only should be movement on y axis
                                                                                            // m_curentParicle.transform.position = move.normalized;
                    m_currentParticle[0].transform.position -= move;

                    if (m_currentParticle[0].transform.position.y < this.transform.position.y) // Check if it has moved past the object.
                    {
                        m_currentParticle[0].Stop();
                        ParticleSystem t = m_currentParticle[0];
                        m_currentParticle.Remove(t);
                        Destroy(t.gameObject, 2);
                        if (m_breakAnimation.Count != 0)
                            if (m_breakAnimation[m_currentProblemIterator] != null)
                            {
                                m_breakAnimation[m_currentProblemIterator].Play();
                            }
                            else
                                m_isPending = false;
                        return true;

                    }
                    return false;
				}
			}
			else if (m_breakAnimation.Count != 0)
				if (m_breakAnimation[m_currentProblemIterator] != null)
					if (m_breakAnimation[m_currentProblemIterator].isPlaying)
					{
						// Does anything need to happen while the animation is playing.
						return false;
					}
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
                                m_buttonPresses--;

                            }
                            else
                            {
                                // Can play an animation on the player to indicate that it's empty.
                            }
                        }
                        else
                        {
                            m_buttonPresses--;
                        }
                        // Get forward vector of player dot product against postion of problem? 
                        // Check if it is within the active 
                        // if player is then rotate directly towards the problem and play swing animation.
                        // Applies - to button presses.
                        if (m_buttonPresses <= 0)
                        {
                            if (item.IsRefillable())
                            {
                                item.UseCharge();
                            }
                            else if (item.IsOneTimeUse())
                            {
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
        // Switching depending on the colliders name
        int count = m_playerList.Count;
        switch (other.name)
        {
            // Each case is the players triggerbox string
            case "Player1":
                m_playerList.Insert(count, Timer.PlayerGet(0)); // Add the correct player to the playerList
                break;
            case "Player2":
                m_playerList.Insert(count, Timer.PlayerGet(1));
                break;
            case "Player3":
                m_playerList.Insert(count, Timer.PlayerGet(2));
                break;
            case "Player4":
                m_playerList.Insert(count, Timer.PlayerGet(3));
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Switching depending on the colliders name
        switch (other.name)
        {
            // Each case is the players triggerbox string
            case "Player1":
                m_playerList.Remove(Timer.PlayerGet(0)); // Remove the correct controller to the playerList
                break;
            case "Player2":              
                m_playerList.Remove(Timer.PlayerGet(1)); // Remove the correct controller to the playerList                
                break;
            case "Player3":             
                m_playerList.Remove(Timer.PlayerGet(2)); // Remove the correct controller to the playerList                
                break;
            case "Player4":
                m_playerList.Remove(Timer.PlayerGet(3)); // Remove the correct controller to the playerList                
                break;
            default:
                break;
        }
    }


    private bool CompareQuality(Item a_item)
    {   
        // Used to compare the item the player currently has against the problems that are currently wrong with it.
        // Currently compares all the qualites that the problem can have. Needs to be changed to the actuve one.
            foreach (E_Quality itemquality in a_item.m_fixableQuality)
                if (m_currentProblem == itemquality)
                    return true;
                else
                    return false;
        return false; // If you hit this then there is a problem.
    }
}