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

    [SerializeField] private GameObject m_inactiveState; // Place the problems inactive state model here.
    [SerializeField] private GameObject m_activeState; // Place the problems active state model here.

    private MeshRenderer m_inactiveMeshRenderer; // Gets the MeshRenderer in startup to turn on/off inactive model.
    private MeshRenderer m_activeMeshRenderer; // Gets MeshRendere in startup to turn on/off active model.


    [SerializeField] private List<E_Quality> m_fixableProblems; // Will be able to set the list size and the problems from the unity editor.
    private E_Quality m_currentProblem; // The problem chosen from the above. Will call particle effects and animation. eg fixable problem 0 on list callse particle effect 0(if it exitss) completes then animation 0(if it exists) then acitvates.
    private int m_currentProblemIterator; // A number that keeps track of what breakparticleeffect/breakanimation/whilebrokenparticleeffect needs to trigger.

    private bool m_isActive = false; // Is the problem active or inactive. Can be used to trigger other events like ui or particle effects.
    private float m_timeBeforeFail; // Once this value is reached the problem will be expired and health will be lost.

    [SerializeField] private int m_maxButtonPresses; // How many wacks it takes to fix the problem. Assaigned every activation.
    private int m_buttonPresses; // The counter for button presses.

    public List<Player> m_playerList; // List of players to check input and item held.

    //*************************************************************************************
    // Extra variables should go here for animation or turning on and off particle effects.
    //*************************************************************************************

    [SerializeField] private List<ParticleSystem> m_breakParticleEffect; // What particle effects take place. 1 to 1 ration with animation and Fixable problem.  // Could probably be changed to game object that is turned on and off.
    [SerializeField] private float m_particleStartDistance = 3; // Where the particel will spawn
    [SerializeField] private float m_particleFallsSpeed = 0.5f; // Controls the speed of the particle falling.
    [SerializeField] private List<Animation> m_breakAnimation; // What happens when it breaks? 1 to 1 ratio with paritcle and fixable problems.
    [SerializeField] private List<ParticleSystem> m_whileBrokenParticle; // Will just loop while the problem is active. 1 to 1.

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


    public bool GetState()
    {
        // Returns if the problem is active or not.
        return m_isActive;
    }

    public void Pending()
    {
        m_currentProblemIterator = Random.Range(0, m_fixableProblems.Count); // Need this int so that we can trigger appropriate animation or paricle effect.
        m_currentProblem = m_fixableProblems[m_currentProblemIterator]; // Sets the quality enum.
        if (m_breakParticleEffect.Count != 0)
            if (m_breakParticleEffect[m_currentProblemIterator].name != "None" || m_breakParticleEffect[m_currentProblemIterator] == null) // Checks if there is a particle effect in here for the associated problem and then activates then returns.
            {
                m_breakParticleEffect[m_currentProblemIterator].Play();
                m_breakParticleEffect[m_currentProblemIterator].transform.position = this.transform.position + new Vector3(0, m_particleStartDistance); // Sets the particle effect to start off sceen above the problem.
            }
        if (m_breakAnimation.Count != 0)
            if (m_breakAnimation[m_currentProblemIterator].name != "None" || m_breakParticleEffect[m_currentProblemIterator] == null || !m_breakParticleEffect[m_currentProblemIterator].isPlaying)
            {
                m_breakAnimation[m_currentProblemIterator].Play(); // This should be enough for the 
            }
    }


    public void Activate(float a_timerBeforeFail)
    {
        m_buttonPresses = m_maxButtonPresses;
        m_isActive = true; // Activate the object
        m_inactiveMeshRenderer.enabled = false; // Hides the problems inactive Mesh.
        m_activeMeshRenderer.enabled = true; // Displays the problems active Mesh.
        m_timeBeforeFail = a_timerBeforeFail;// Assaigns it a time frame before a life is lost.
        //Add itself to the bottom of the list. - Will need to be done from m_problemList
        //Remove itself from current position in list. - Will need to be done from m_problemList
    }

    public void Deactivate()
    {
        m_isActive = false;
        m_inactiveMeshRenderer.enabled = true;// Displays the problem inactive Mesh.
        m_activeMeshRenderer.enabled = false; // Hides the problem active Mesh.
        // Add itself to the top of the list. - Will need to be done from m_problemList
        // Removes itself from current postion in list. - Will need to be done from m_problemList
    }

    public bool CheckPending()
    {
        
        if (m_breakParticleEffect[m_currentProblemIterator].name != "None" || m_breakParticleEffect[m_currentProblemIterator] == null)
            if (m_breakParticleEffect[m_currentProblemIterator].isPlaying) // Specific case for particle effect as upon completion 
            {
                Vector3 move = new Vector3(0, 1 * m_particleStartDistance, 0); // Only should be movement on y axis
                m_breakParticleEffect[m_currentProblemIterator].transform.forward = move.normalized;
                m_breakParticleEffect[m_currentProblemIterator].transform.position += move;

                if (m_breakParticleEffect[m_currentProblemIterator].transform.position.y < this.transform.position.y) // Check if it has moved past the object.
                {
                    m_breakParticleEffect[m_currentProblemIterator].Stop(); 
                    if (m_breakAnimation[m_currentProblemIterator].name != "None") 
                        m_breakAnimation[m_currentProblemIterator].Play(); // Activate the animation.
                }
                return false;
            }
        if (m_breakAnimation[m_currentProblemIterator].name != "None" || m_breakParticleEffect[m_currentProblemIterator] == null)
            if (m_breakAnimation[m_currentProblemIterator].isPlaying)
            {
                // Does anything need to happen while the animation is playing.
                return false;
            }
        return true;
    }


    public bool CheckActive(float a_timeLimit)
    {
        //@Checks the state of currently activated objects. If expired return true.
        if (m_timeBeforeFail >= a_timeLimit && m_isActive)
        {
            this.Deactivate(); // Deactivates itself
            return true; // Take damage.
        }
        else
            return false; // Don't take damage.
    }

    private void Actions()
    {
        for (int i = 0; i < m_playerList.Count; i++)
        {
            XboxController controller = m_playerList[i].controller;
            if (XCI.GetButtonDown(XboxButton.A, controller))
                if (CompareQuality(m_playerList[i].GetItem())) // Needs to check rotation if facing problem
                {
                    // Get forward vector of player dot product against postion of problem? 
                    // Check if it is within the active 
                    // if player is then rotate directly towards the problem and play swing animation.
                    // Applies - to button presses.
                    m_buttonPresses--;
                    if (m_buttonPresses <= 0)
                   {
                        Deactivate();
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
        foreach (E_Quality problemquality in this.m_fixableProblems)
            foreach (E_Quality itemquality in a_item.m_fixableQuality)
                if (problemquality == itemquality)
                    return true;
                else
                    return false;
        return false; // If you hit this then there is a problem.
    }
}