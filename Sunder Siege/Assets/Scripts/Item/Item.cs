﻿/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Controls the items input handling 
 * Data Created: 12th Sep, 2019
 * Last Modified: 24st Sep, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

// @detail The item class is in charge of handling the different inputs from
//		   the players. Depending on that input an action (pickup, drop, swing, throw) 
//		   will take place. The only times these actions will occur is when 1, a player
//		   is near the item AND 2, the player inputted the correct button (Y, B, A, X).
//		   These aforementioned actions are within this class instead of the player class
//		   so that the system is scalable, reuseable, and modular. In theory any GameObject
//		   can now be manipulated by the player using this script.
public class Item : MonoBehaviour
{
	/* Variables that store Player base data */
	public List<Player> m_playerList;                           // Get what player is doing the actions -- FOR SOME REASON YOU CAN NO LONGER ADD TO THIS LIST!? I've changed all the m_playerLists to insert at count... works fine... for now.
	//SerializeField] private Animator m_swingAnim;              // Swinging arm animation -- Needs to be changed to an enum of the animation that is stored within the player. When input is


	/* Variables that store Item base data */
	[SerializeField] private float m_throwForce = 1;            // How much force will be applied to the item when thrown
	[SerializeField] private List<BoxCollider> m_model;         // The models of the item to turn off so player can walk over unhindered -- Can probably turn this into a tag detection that makes the item ignore players for detection.
	private Rigidbody m_rigidbody;                              // The rigid body on the item so it reacts to gravity. Needs to be made kinematic to stop the item from falling through the world -- If above can be done this may not be needed.
	[SerializeField] private Vector3 m_raycastOffset;
	private Vector3 m_hitLocation;
	private bool m_isThrown = false;
	private Vector3 m_lastPosition;
	private Vector3 m_currentPosition;


	private Vector3 m_lastPos;                                  // Store the items previous position    
	private Player m_playerHolding = null;                      // The player holding the item.

	[SerializeField] private Vector3 m_itemOffset;              // Where the item is held relative to the player once picked up.
																//[SerializeField] private Vector3 lowerCastOffset;

	/* Variables that store if the item is reuseable */
	[SerializeField] private bool m_refillable = false;         // Tells if the player needs to go to a place to refill.
	[SerializeField] private int m_maxCharges;                  // How many time the item can be used before it needs to refilled.
	[SerializeField] private bool m_losesChargesOnThrow = false;// Does the item lose charges if it is thrown.
    [SerializeField] private bool m_oneTimeUse = false;         // Will be destroyed upon use.
    private int m_charges;                                      // How many time the item can be used before it needs to refilled.
	private bool m_wasRefilled = false;
    private Source m_spawner;                                   // Upon spawn will assaign the item to a spawner so it knows who needs to destroy it.

	/* Misc varaibles */
	public List<E_Quality> m_fixableQuality; // Holds what the item can fix and if it can fix more then 1 type of problem eg; it is both a hammer and a bucket.




	// @brief Runs at the start of the games launch.
	//		  sets to details of the rigidbody varaible.
	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		m_charges = m_maxCharges;
	}

	// @brief Update is called once per frame.
	//		  Makes sure there is a player in range before checking any actions
	void Update()
	{

		if (m_isThrown)
		{
			BeepBoopRotateSoup();
		}

		if (m_playerList != null)
		{
			if (m_playerList.Count > 0 || m_playerHolding != null)
			{
				Actions();
			}
		}
		m_wasRefilled = false;
	}

	// @brief Checks when another collider has entered the trigger area
	//		  Depending on the name of the collider, we can tell what
	//		  player is within range of the item
	private void OnTriggerEnter(Collider other)
	{
		// Switching depending on the colliders name
		int count = m_playerList.Count;
		Player collidedPlayer = null;
		switch (other.name)
		{
			// Each case is the players triggerbox string
			case "Player1":
				collidedPlayer = Timer.PlayerGet(0);
				m_playerList.Insert(count, collidedPlayer); // Add the correct player to the playerList
				break;
			case "Player2":
				collidedPlayer = Timer.PlayerGet(1);
				m_playerList.Insert(count, Timer.PlayerGet(1));
				break;
			case "Player3":
				collidedPlayer = Timer.PlayerGet(2);
				m_playerList.Insert(count, Timer.PlayerGet(2));
				break;
			case "Player4":
				collidedPlayer = Timer.PlayerGet(3);
				m_playerList.Insert(count, Timer.PlayerGet(3));
				break;
			default:
				break;
		}

		// Unfreeze the Y when the item is in a collision area with another object
		m_rigidbody.constraints = RigidbodyConstraints.None;


		// Check if the item is currently in the air
		if (collidedPlayer /*m_isThrown*/)
		{
			// Check that whats collided is a player
			if (m_isThrown /*collidedPlayer*/)
			{
				// Check that the player isn't already holding an item
				if (m_playerHolding)
				{
					// Pickup the item
					Pickup(collidedPlayer);
				}
			}
			else
			{
				foreach (BoxCollider model in m_model)
				{
					Physics.IgnoreCollision(model.GetComponent<BoxCollider>(), collidedPlayer.GetComponent<BoxCollider>());
				}
			}
		}


		// Old system
		//if (m_playerList != null)
		//	if (m_playerList.Count > 0)
		//	{
		//		// Make kinematic and turn off box colliders on shaft and head. With whole models this will be easier.
		//		// Item will not fall through world or cause player to walk on it.
		//		m_rigidbody.isKinematic = true;
		//		foreach (BoxCollider collider in m_model)
		//		{
		//			collider.enabled = false;
		//		}
		//	}

		//if (m_isThrown == false)
		//	foreach (Player player in m_playerList)
		//		if (player.GetItem() == null)
		//		{
		//			m_rigidbody.isKinematic = true;
		//			foreach (BoxCollider collider in m_model)
		//			{
		//				collider.enabled = false;
		//			}
		//		}
	}

	// @brief Checks when another collider has exited the trigger area
	//		  Depending on the name of the collider, we can tell what
	//		  player is out of range of the item
	private void OnTriggerExit(Collider other)
	{
		Player collidedPlayer = null;
		// Switching depending on the colliders name
		switch (other.name)
		{
			// Each case is the players triggerbox string
			case "Player1":
				collidedPlayer = Timer.PlayerGet(0);
				m_playerList.Remove(collidedPlayer); // remove the correct player from the playerList
				break;
			case "Player2":
				collidedPlayer = Timer.PlayerGet(1);
				m_playerList.Remove(collidedPlayer);
				break;
			case "Player3":
				collidedPlayer = Timer.PlayerGet(2);
				m_playerList.Remove(collidedPlayer);
				break;
			case "Player4":
				collidedPlayer = Timer.PlayerGet(3);
				m_playerList.Remove(collidedPlayer);
				break;
			default:
				break;
		}

		if (collidedPlayer)
		{
			if (m_playerHolding != true)
			{
				foreach (BoxCollider model in m_model)
				{
					Physics.IgnoreCollision(model.GetComponent<BoxCollider>(), collidedPlayer.GetComponent<BoxCollider>(), false);
				}
			}
		}

		// Old system.
		//if (m_playerList.Count == 0)
		//{
		//	// Item will not fall through world or cause player to walk on it.
		//	m_rigidbody.isKinematic = false; // No lnger kinematic and turn on box colliders of models with multiple peices. Eg: shaft and head of hammer. 
		//	foreach (BoxCollider collider in m_model)
		//	{
		//		collider.enabled = true;
		//	}
		//}
	}

	// @brief Handles the different inputs the player can press.
	//		  Depending on the input, the item can be picked up,
	//		  dropped, thrown, and swung.
	public void Actions()
	{
		XboxController controller;
		if (m_playerHolding != null && m_isThrown == true) // A check to make sure player can't hold a thrown item.
			m_isThrown = false;

		if (m_playerHolding == null)
		{
			for (int i = 0; i < m_playerList.Count; i++)
			//foreach (Player player in m_playerList)
			{
				if (!m_isThrown)
				{
					controller = m_playerList[i].controller;

					// If the item is already being held, go into swinging logic
					// otherwise go into picking up logic.
					if (XCI.GetButtonDown(XboxButton.A, controller) && m_playerList[i].GetItem() == null)
					{
						Pickup(m_playerList[i]); // Player will pick it up.
					}
				}
				else
				{
					if (m_playerList[i].GetItem() == null)
					{
						//m_playerHolding = m_playerList[i];    // Moved into pickupfunction           
						Pickup(m_playerList[i]);
					}
				}
			}
		}
        //else if (m_isThrown)
        //{
        //    MoveTowards();
        //}
        else 
		{
			controller = m_playerHolding.controller;
			//if (XCI.GetButtonDown(XboxButton.A, controller))
			//{
			//	SwingTrue();
			//}
			// If the item is in the players hand then handle dropping the item when 'B' is pressed
			if (XCI.GetButtonDown(XboxButton.B, controller))
			//if(Input.GetKeyDown(KeyCode.A)) // Just so you can pick up with keyboard
			{
				Drop();
				//Needs to get
			}
			// If the item is in the players hand then handle throwing the item when 'Y' is pressed
			if (XCI.GetButtonDown(XboxButton.Y, controller))
			{
				Throw();
			}
		}
	}

	public void Pickup(Player a_player)
	{
        // Refer to player class for Pickup
        m_playerHolding = a_player;
        m_playerHolding.PickUpItem(this);

		foreach (BoxCollider model in m_model)
		{
			Physics.IgnoreCollision(model.GetComponent<BoxCollider>(), m_playerHolding.GetComponent<BoxCollider>(), false);
		}
	}

	void Drop()
	{
		// Refer to player class for drop
		m_playerHolding.DropItem();
		m_playerHolding = null;
		m_rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);

	}

	void Throw()
	{
		// Freeze the Y position to keep the item from dropping down
		m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;

		// Work out where the throw will go. (Raycast). Will need to make a tag that players will able to throw over. The ray cast will simply ignore them.
		RaycastHit vision = new RaycastHit();

		// The object then needs to move along that line every frame and check if a player has entered it's hitbox while facing and Without an item.

		Debug.DrawRay(m_playerHolding.transform.position + m_raycastOffset, m_playerHolding.transform.forward, Color.red, 1f);

		if (Physics.Raycast(m_playerHolding.transform.position + m_raycastOffset, m_playerHolding.transform.forward, out vision, 9999f))
		{
			Debug.Log(vision.collider.name);
			m_rigidbody.AddForce(transform.forward * m_throwForce, ForceMode.Impulse);
			m_isThrown = true;

			// Old system
			//m_hitLocation = vision.point;
			//m_isThrown = true;
			//m_playerList.Clear();
			//m_rigidbody.isKinematic = true;
			//m_rigidbody.useGravity = false;

			//MoveTowards();
		}
		m_playerHolding.ThrowItem(); // Might need to be adjusted at the player class since player class currently drops items at "feet"
		m_playerHolding = null;

		//foreach (BoxCollider collider in m_model)
		//{
		//	collider.enabled = true;
		//}

		if (m_losesChargesOnThrow)
		{
			m_charges = 0;
		}
	}

	void BeepBoopRotateSoup()
	{
		if (m_lastPosition == m_currentPosition)
		{
			m_isThrown = false;
		}
		else
		{
			float tiltAroundZ = this.transform.position.y * 500f;
			Quaternion target = Quaternion.Euler(tiltAroundZ, 0, tiltAroundZ);
			transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 50.0f);
		}

	}

	public Vector3 GetOffset()
	{
		// The postion offset. This is used to place the item at a certain point on the player without parenting it to a place. Once modesl are in game and not scaled parenting will be a better choice
		return m_itemOffset;
	}

	public void RefillCharges()
	{
		// I refills the items charges. Might not need the bool. Should check above logic for when a swing happens.
		m_wasRefilled = true;
		m_charges = m_maxCharges;
	}

	public int GetCharges()
	{
		// Returns items current charges. Should really only be called by problems to confirm if the 
		return m_charges;
	}

	public bool IsRefillable()
	{
		return m_refillable;
	}

	//Will return true if a charge has been used.
	public void UseCharge()
	{
		// Will deduct charges from item. Could probably be change to a bool keeping logic in one place
		if (m_charges > 0)
		{
			m_charges--;
		}
	}

    public bool GetHeldItem() 
    {
        // Returnsfalse if item is not held.
        return (m_playerHolding == null);   
    }

    public bool IsOneTimeUse()
    {
        // Tells you if 1 time use
        return m_oneTimeUse;
    }

    public void SetSpawn(Source a_spawner)
    {
        m_spawner = a_spawner;
    }

    public Source GetSpawner()
    {
        return m_spawner;
    }
}


/* Collision Rules
 * 1. If a player eneters the collisions, it needs to ignore the players collision (not collide with player). If it is NOT thrown, and IF the palyer is not currently holding an item
 *			- Never adds the player to the item list, players may need to still be added to the list, but it needs to ignore the player
 *			
 * Check if the player is in the list
 * Then check what the player is doing, holding an item, facing an item, etc.
 * 
 * 
 * 
 * 
 * OnTriggerEnter()
 * check if it's a player and add it to it's list.
 *		if the item is thrown.
 *			renebale Y
 *			if the player is facing(when that gets implemtned) && if the player is not holding an item
 *				the player picks up item
 *			else
 *				disable collider on player entered
 * else
 *		if (y) is frozen
 *			renebale.
 *		
 */

/* Pickup and Drop changes
 * Disable the collider with the player thats picked it up, remove clear.
 * Change Drop to add a force forward to move it forward a bit. Idea, drop it in front
 */

/* Throw changes
 * Make it travel along the Y (freeze Y?)
 * Addforce rather than transform, huge amount but only once
 */