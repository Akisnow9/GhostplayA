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
    public List<PhysicsObjectPositionalData> m_childrenStartPostions; // Postion of all the children.

    

    [SerializeField] private float m_addedForce; // The force applied to all objects. -- Does it need to be applied to parents or each part.
    [SerializeField] private float m_timeBeforeDissapear; // How long the objects will exist in the game world
    [SerializeField] private float m_scaleSpeedOfChildren; // Passed into the setup for children objects.
    // May not need below colliderlist.
    [SerializeField] private List<GameObject> m_children; // Colliders of all the children too ignore. -- If more is needed to be done could be gameobjects.
    private Vector3 m_postion; // Instead of creating and destroying a vector3 just have one always available to assaign.

    [SerializeField] private bool m_dissapear = false;

    [SerializeField] private ParticleSystem m_dissapearingParticleEffect; // The pieces will dissapear with a particle effect. -- Might change into a list.
    
    // private Shader m_dissapearingShaderEffect;

    private bool m_isDoingPhysics; // If yes will check if it is time to destroy itself in update.
    private float m_timeToDissapear; // When the peice will deactivate.
    private PhysicsObjectPositionalData m_startPostion; // Reset postion. Will return to this once complete. Might not be enough because of the way items are parented. Might need to also store rotation.

    void Start()
    {
        m_childrenStartPostions = new List<PhysicsObjectPositionalData>();
        // NEED TO MAKE SURE ALL THE PHYSIC PARTS IGNORE PLAYERS.
        // May need to disable all physics interactions until apply force is called.
        for (int i = 0; i < Timer.PlayerAmountGet(); i++) // Cycles through all players and sets them to ignore each of the physics problems.
            foreach (GameObject child in m_children) // cycles through all children -- This is extremly dumb. Should just put a check which ignores tag 'player' in oncollisonenter.
            {
                if (child.GetComponent<ChildrenPhysicsObject>() == null)
                {
                    child.AddComponent<ChildrenPhysicsObject>();
                }
                if (child.GetComponent<Collider>() == null)             // Checks if the child has a box collider. If not adds one.
                    child.AddComponent<BoxCollider>(); 
                Physics.IgnoreCollision(child.GetComponent<Collider>(), Timer.PlayerGet(i).GetComponent<Collider>()); // Should ensure no interaction between palyer and parent.. Does this extend to children?
            }

        foreach(GameObject child1 in m_children)
            foreach(GameObject child2 in m_children)
            {
                Physics.IgnoreCollision(child1.GetComponent<Collider>(), child2.GetComponent<Collider>());
            }
        // May need to add a null check in case of no children.
        for (int i = 0; i < m_children.Count; i++)
        {
            m_startPostion = new PhysicsObjectPositionalData();
            m_startPostion.m_position = m_children[i].transform.position;
            m_startPostion.m_rotation = m_children[i].transform.rotation;
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
        //this.gameObject.SetActive(false);
    } // Ignores collsion of player and other children of object. Saves start postion data.


    // Update is called once per frame
    void Update()
    {
       
    }

    public void ApplyForce() 
    {
        // Called when physics needs to be applied. Once the models are swapped this is then called.
        this.gameObject.SetActive(true);
        m_isDoingPhysics = true; // Now true it will check if it is destroyed every frame.
        foreach(GameObject child in m_children)
        {
            child.gameObject.SetActive(true);
            if(child.GetComponent<Rigidbody>() == null) // Checks if the child has a rigidbody. If not adds one.
            {
                child.AddComponent<Rigidbody>(); 
            }
            child.GetComponent<Rigidbody>().AddForce(transform.forward * (m_addedForce + (float)Random.Range(0, 10)) , ForceMode.Impulse); // Adds force to each individual object.
        }

        // Set the time time for when it is to be "destroyed".
        if (m_dissapear)
        {
            m_timeToDissapear = Timer.TimeGet() - m_timeBeforeDissapear; // Will dissaper 'x' amount of time in the future.
            foreach (GameObject child in m_children)
            {
                child.GetComponent<ChildrenPhysicsObject>().Setup(m_timeToDissapear, m_scaleSpeedOfChildren);
            }
        }
    }

    public void ResetToStartPosition()
    {
        // Sets postions back to start
        m_isDoingPhysics = false; // No longer active so will not update.
        for (int i = 0; i < m_children.Count; i++)
        {
            m_children[i].transform.rotation = m_childrenStartPostions[i].m_rotation; // Sets child postion back to start.
            m_children[i].transform.position = m_childrenStartPostions[i].m_position;
            m_children[i].transform.localScale = new Vector3(1, 1, 1);
            m_children[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); // Sets velocity to 0.
        }
        this.gameObject.SetActive(false);
    }
}
