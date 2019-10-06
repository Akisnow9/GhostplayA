/***************************************************
 * Written By: Anton Huber
 * Purpose: Applies force to the physics objects 
 * Data Created: 06/10
 * Last Modified: 06/10
 **************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemPhsyicsObject : MonoBehaviour
{
    //*************************************************************************************
    // Once the problem activates some objects will need to swap and then add a physics reaction.
    // eg: Wall has hole punched into it so after the wall swap happens physics is applied to the
    // peices. These peices will be non interactible. Thye will phase out after a time limit.
    // Will need to reset to start postion once fixed.
    //*************************************************************************************
    public List<Vector3> m_childrenStartPostions; // Postion of all the children.

    [SerializeField] private float m_addedForce; // The force applied to all objects. -- Does it need to be applied to parents or each part.
    [SerializeField] private float m_timeBeforeDissapear; // How long the objects will exist in the game world
    
    // May not need below colliderlist.
    [SerializeField] private List<GameObject> m_children; // Colliders of all the children too ignore. -- If more is needed to be done could be gameobjects.
    private Vector3 m_postion; // Instead of creating and destroying a vector3 just have one always available to assaign.

    [SerializeField] private ParticleSystem m_dissapearingParticleEffect; // The pieces will dissapear with a particle effect.
    
    // private Shader m_dissapearingShaderEffect;

    private bool m_isDoingPhysics; // If yes will check if it is time to destroy itself in update.
    private float m_timeToDissapear; // When the peice will deactivate.
    private Vector3 m_startPostion; // Reset postion. Will return to this once complete. Might not be enough because of the way items are parented. Might need to also store rotation.

    void Start()
    {
        // NEED TO MAKE SURE ALL THE PHYSIC PARTS IGNORE PLAYERS.
        // May need to disable all physics interactions until apply force is called.
        for (int i = 0; i < Timer.PlayerAmountGet(); i++) // Cycles through all players and sets them to ignore each of the physics problems.
            foreach (GameObject child in m_children) // cycles through all children -- This is extremly dumb. Should just put a check which ignores tag 'player' in oncollisonenter.
            {
                Physics.IgnoreCollision(child.GetComponent<BoxCollider>(), Timer.PlayerGet(i).GetComponent<BoxCollider>()); // Should ensure no interaction between palyer and parent.. Does this extend to children?
            }

        foreach(GameObject child1 in m_children)
            foreach(GameObject child2 in m_children)
            {
                Physics.IgnoreCollision(child1.GetComponent<BoxCollider>(), child2.GetComponent<BoxCollider>());
            }
        // May need to add a null check in case of no children.
        for (int i = 0; i < m_children.Count; i++)
        {
            m_startPostion = m_children[i].transform.position;
            if (m_childrenStartPostions != null)
            {
                int count = m_childrenStartPostions.Count;
                m_childrenStartPostions.Insert(count, m_startPostion); // Might need to change to insert.
            }
            else
            {
                m_childrenStartPostions.Insert(0, m_startPostion);
            }
        }
        this.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if(m_isDoingPhysics)
        {
            if(Timer.TimeGet() > m_timeToDissapear) // Checks if it's time to dissapear
            {
                
                // Does it dissapear via shader? - Does it dissapear via particle?
            }
        }
        // Nothing else should happen other then physics which is handled by engine.
    }

    public void ApplyForce() 
    {
        // Called when physics needs to be applied. Once the models are swapped this is then called.
        this.gameObject.SetActive(true);
        m_isDoingPhysics = true; // Now true it will check if it is destroyed every frame.
        foreach(GameObject child in m_children)
        {
            child.GetComponent<Rigidbody>().AddForce(transform.forward * m_addedForce, ForceMode.Impulse); // Adds force to each individual object.
        }

        // Set the time time for when it is to be "destroyed".
        m_timeToDissapear = Timer.TimeGet() + m_timeBeforeDissapear; // Will dissaper 'x' amount of time in the future.
    }

    public void ResetToStartPosition()
    {
        // Sets postions back to start
        m_isDoingPhysics = false; // No longer active so will not update.
        for (int i = 0; i < m_children.Count; i++)
        {
            m_children[i].transform.position = m_childrenStartPostions[i]; // Sets child postion back to start.
        }
        this.gameObject.SetActive(false);
    }
}
