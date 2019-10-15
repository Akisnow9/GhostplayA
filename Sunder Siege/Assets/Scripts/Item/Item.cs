/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Controls the items input handling 
 * Data Created: 12th Sep, 2019
 * Last Modified: 14th Oct, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

// @detail The item class is in charge of handling the different inputs from
//		   the players. Depending on that input an action (pickup, drop, throw) 
//		   will take place. The only times these actions will occur is when 1, a player
//		   is near the item AND 2, the player inputted the correct button (A, B, Y, X).
//		   These aforementioned actions are within this class instead of the player class
//		   so that the system is scalable, reuseable, and modular. In theory any GameObject
//		   can now be manipulated by the player using this script.
public class Item : MonoBehaviour
{
	/* Variables that store Player base data */
	[Header("DON'T TOUCH THIS PLAYER LIST")]
	public List<Player> m_playerList;                           // Get what player is doing the actions
	private Player m_playerHolding = null;                      // Stores the player currently holding the item
	private Player m_lastPlayerHolding = null;                  // Stores the previous player holding the item

	/* Variables that store Item base data */
	[Header("Mesh that contains the collider")]
	[SerializeField] private List<Collider> m_model;            // The models of the item to turn off so player can walk over unhindered
	[Header("Offset of the item when picked up")]
	[SerializeField] private Vector3 m_itemOffset;              // Where the item is held relative to the player once picked up
	[Header("Offset on how high the item is thrown")]
	[SerializeField]
	[Range(0.3f, 0.8f)] private float m_rayOffset;              // Offset of where the raycast is shot from
	[Header("How hard the item is thrown")]
	[SerializeField] private float m_throwForce = 0.5f;         // How much force will be applied to the item when thrown
	[Header("How much the item bounces when it collides with somethiing")]
	[SerializeField] private float m_bounceForce = 5f;          // How much force will be applied to the item when ricochet
	private Vector3 m_hitLocation;								// Location the raycast hit when the item is thrown
	private Vector3 m_currentPosition;							// Stores the currently position (transform) of the item
	private Vector3 m_lastPosition;								// Stores the previous position (transform) of the item
	private Vector3 newEulerAngles;								// Stores new eular angles used for rotating the item when it is thrown
	private Rigidbody m_rigidbody;                              // RB for general Physics things, applying force, toggling gravity, etc.
	private bool m_isThrown = false;                            // Stores if the item is currently airborne 
	private bool m_isDropped = false;                           // Stores if the item is currently dropping

	/* Variables that store if the item is reuseable */
	[Header("If an item is one time use, destroyed on use")]
	[SerializeField] private bool m_oneTimeUse = false;         // Will be destroyed upon use
	[Header("If the item is to lose its charges on thrown (buckets, etc)")]
	[SerializeField] private bool m_losesChargesOnThrow = false;// Does the item lose charges if it is thrown
	[Header("If the item is refillable at a souce")]
	[SerializeField] private bool m_refillable = false;         // Tells if the player needs to go to a place to refill
	[Header("The different levels within the items")]
	[SerializeField] private List<GameObject> m_filledLevels;   // Stores the different levels of "filled"
	[Header("Max number of charges the item has to repair problems")]
	[SerializeField] private int m_maxCharges;                  // How many time the item can be used before it needs to refilled
	private Source m_spawner;                                   // Upon spawn will assaign the item to a spawner so it knows who needs to destroy it
	private int m_charges;                                      // How many time the item can be used before it needs to refilled
	private bool m_wasRefilled = false;                         // Stores if the item was already refilled

	/* Misc varaibles */
	[Header("What the actual item is, bucket, hammer, etc.")]
	public List<E_Quality> m_fixableQuality; // Holds what the item can fix and if it can fix more then 1 type of problem eg; it is both a hammer and a bucket.

	private float m_count;

	//*************************************************************************************
	// Base class functions
	//*************************************************************************************
	// @brief Runs at the start of the games launch
	//		  Sets details about the rigidbody variable and the charges count
	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		m_charges = m_maxCharges;
		m_count = m_filledLevels.Count;
	}

	// @brief Update is called once per frame
	//		  Handles moving the item when it is thrown
	//		  Makes sure there is a player in range before checking any actions
	void Update()
	{
		// If the item is refillable, do the maths to deduct each segment when used
		if (m_refillable)
		{
			ItemDeduct();
		}

		// Check if the item is thrown
		if (m_isThrown)
		{
			// Make sure no player is holding the item
			if (m_playerHolding == false)
			{
				// Head towards target location
				MoveTowards();
			}
		}

		// Check that there is data within the playerList
		if (m_playerList != null)
		{
			// Make sure the playerList is larger than zero
			if (m_playerList.Count > 0)
			{
				// Allow for actions to take place
				Actions();
			}
		}

		// Item was no longer refilled at the end of a frame
		m_wasRefilled = false;
	}

	//*************************************************************************************
	// Player and Item core functionality
	//*************************************************************************************
	// @brief Handles the different inputs the player can press
	//		  Depending on the input, the item can be picked up,
	//		  dropped, and thrown
	public void Actions()
	{
		// Have a controller enum stored
		XboxController controller;

		// A check to make sure player can't hold a thrown item
		if (m_playerHolding != null && m_isThrown)
		{
			m_isThrown = false;
		}

		// If there is no player holding the item
		if (m_playerHolding == null && !m_isThrown)
		{
			// Loop through each player near the item (each player in the List)
			for (int i = 0; i < m_playerList.Count; i++)
			{
				// Set the enum on the controller to the player thats in ranges controller
				controller = m_playerList[i].controller;

				// if the 'A' button is pressed, and an item isn't already held (this is for swing logic overloading the 'A' button)
				// then pickup the item.
				if (XCI.GetButtonDown(XboxButton.A, controller) && m_playerList[i].GetItem() == null)
				{
					Pickup(m_playerList[i]);
				}
			}
		}
		else if (m_playerHolding != null)// Otherwise there is a player holding the item
		{
			// The controller is assigned the player holding the item controller
			controller = m_playerHolding.controller;

			// If the item is in the players hand then handle dropping the item when 'B' is pressed
			if (XCI.GetButtonDown(XboxButton.B, controller))
			{
				Drop();
			}

			// If the item is in the players hand then handle throwing the item when 'Y' is pressed
			if (XCI.GetButtonDown(XboxButton.Y, controller))
			{
				Throw();
			}
		}
	}

	// @brief Handles picking up the item when called
	// @param Player class to pass through the player that is grabbing the item
	public void Pickup(Player a_player)
	{
		// Refer to player class for Pickup
		m_playerHolding = a_player;
		m_lastPlayerHolding = m_playerHolding;
		m_playerHolding.PickUpItem(this);
	}

	// @brief Handles dropping the item when called
	void Drop()
	{
		m_isDropped = true;
		// Item will not fall through world or cause player to walk on it
		m_rigidbody.isKinematic = false;

		foreach (Collider Collider in m_model)
		{
			Collider.enabled = true;
		}

		m_rigidbody.AddForce(transform.forward * 1.5f, ForceMode.Impulse);

		// Refer to player class for drop
		m_playerHolding.DropItem();
		m_playerHolding = null;
	}

	// @brief Handles throwing the item, the location to be throwin towards based
	//        off of a raycast from the player position in a straight line
	void Throw()
	{
		// Store a raycasthit class to be able to get information about where the cast landed
		RaycastHit vision = new RaycastHit();

		// Free the Y position so that the item does not move when it is grabbed and thrown
		m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;

		Vector3 rayOffset = new Vector3(0, m_rayOffset, 0);
		Debug.DrawRay(m_playerHolding.transform.position + rayOffset, m_playerHolding.transform.forward, Color.black, 5f);

		// If the raycast has actually hit something
		if (Physics.Raycast(m_playerHolding.transform.position + rayOffset, m_playerHolding.transform.forward, out vision, 9999f))
		{
			// Output the name of the object collided
			Debug.Log(vision.collider.name);

			// Set the hitLocation to the point of impact
			m_hitLocation = vision.point;

			m_isThrown = true;

			// Begin to ignore collisions between the model and the last player that held the item
			for (int i = 0; i < m_model.Count; i++)
			{
				Physics.IgnoreCollision(m_model[i], m_lastPlayerHolding.GetComponent<Collider>(), true);
			}

			// When no longer uses gravity to keep it going in a straight line
			m_rigidbody.useGravity = false;

			// Begin moving towards the hitLocation
			MoveTowards();
		}

		// Refer to the player class to throw the item, also have no player holding the item anymore
		m_playerHolding.ThrowItem();
		m_playerHolding = null;

		// Item will not fall through world or cause player to walk on it.
		m_rigidbody.isKinematic = false; 

		// Begin to enable all the colliders to allow it to ricochet off of objects
		foreach (Collider Collider in m_model)
		{
			Collider.enabled = true;
		}

		// If the item is to lose its charges when thrown, set its chargers to zero

	}

	// @brief Handles moving along a line towards the hitLocation retrieved above via
	//        the raycast. Also rotates the item whilst it is moving
	void MoveTowards()
	{
		// Remember the last position
		m_lastPosition = transform.position;

        // Modify the position via transform.position
        transform.position = Vector3.MoveTowards(transform.position, m_hitLocation, m_throwForce);
        //m_rigidbody.AddForce(transform.forward * m_throwForce, ForceMode.Impulse);

		// Set the new current position
		m_currentPosition = transform.position;

		// Set the new eular angles to the current transform, multiplying the Y to have it spin
		newEulerAngles = transform.position;
		newEulerAngles.y = transform.position.y * 500f;

		// Set the target to be the euler of the new angles
		Quaternion target = Quaternion.Euler(newEulerAngles.x, newEulerAngles.y, newEulerAngles.z);

		// Gradually slerp towards te target over a set amount of time
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 50f);

		// If the items last postion is equal to the current position, its no longer moving
		if (m_lastPosition == m_currentPosition)
		{
			// Reset the varaibles to their default as the item is no longer airborne
			m_isThrown = false;
			m_rigidbody.isKinematic = false;
			m_rigidbody.useGravity = true;

			// Re-enable the colliders in the model
			foreach (Collider Collider in m_model)
			{
				Collider.enabled = true;
			}
		}
	}

	// @brief Handles deducting levels from the item when it is used. This is done by
	//		  percentage mathematics for the amount of levels there are
	public void ItemDeduct()
	{
		// Ignore zero
		if (m_count != 0)
		{
			float number = (float)GetCharges() / (float)GetMaxCharges();
			float number2 = (float)m_count / (float)m_filledLevels.Count;

			if (number < number2)
			{
				m_filledLevels[(int)(m_filledLevels.Count - m_count)].SetActive(false);

				m_count--;
				if (m_count != 0)
				{
					m_filledLevels[(int)(m_filledLevels.Count - m_count)].SetActive(true);
				}
			}
		}
		else
		{
			for (int i = 0; i < m_filledLevels.Count - 1; i++)
			{
				m_filledLevels[i].SetActive(false);
			}
		}
	}

	// @brief Resets the items levels to the max level
	public void ItemReset()
	{
		// Reset the count
		m_count = m_filledLevels.Count;

		// Reset the levels
		for (int i = 0; i < m_filledLevels.Count - 1; i++)
		{
			if (i <= 0)
			{
				m_filledLevels[i].SetActive(true);
			}
			else
			{
				m_filledLevels[i].SetActive(false);
			}
		}
	}

	//*************************************************************************************
	// Source based items functionality
	//*************************************************************************************
	// @brief Handles using a charge on an item when swung
	public void UseCharge()
	{
		// Will deduct charges from item as long as there are more than zero charges left
		if (m_charges > 0)
		{
			m_charges--;
		}
	}

	// @brief Handles refilling the charges of the item when called
	public void RefillCharges()
	{
		// Reset the charges to the max
		m_charges = m_maxCharges;
	}

	//*************************************************************************************
	// Getters and Setters
	//*************************************************************************************
	// @brief Handles setting the source spawner gameObjects
	// @param Passes in a Source class to hold the data for spawner
	public void SetSpawn(Source a_spawner)
	{
		m_spawner = a_spawner;
	}

	// @brief Gets if the item is only one time use
	// @return Returns a bool on if the item is one time use or not
	public bool IsOneTimeUse()
	{
		return m_oneTimeUse;
	}

	// @brief Gets the item is refillable at sources
	// @return Returns a bool on if the item is refillable or not
	public bool IsRefillable()
	{
		return m_refillable;
	}

	// @brief Gets the amount of charges the item has left when called
	// @return Returns an int with the data of how many charges are left
	public int GetCharges()
	{
		// Returns items current charges
		return m_charges;
	}

	public int GetMaxCharges()
	{
		return m_maxCharges;
	}

	// @brief Gets the spawner to see what the source is
	// @return Returns a Source to see what type of spawner it is
	public Source GetSpawner()
	{
		return m_spawner;
	}

	// @brief Gets the playerHolding to see if there is an item being held
	// @return Returns a bool on if an item is held or not
	public bool GetHeldItem()
	{
		// Returns false if item is not held.
		return (m_playerHolding == null);
	}

	// @brief Gets the positional offset of the item
	// @return Returns a Vector3 with the item offset X, Y, and Z
	public Vector3 GetOffset()
	{
		// The postion offset. This is used to place the item at a certain point on the player without parenting it to a place
		return m_itemOffset;
	}

	//*************************************************************************************
	// OnTrigger and OnCollision event functionality
	//*************************************************************************************
	// @brief Checks when another Collider has entered the trigger area
	//		  Depending on the name of the Collider, we can tell what
	//		  player is within range of the item
	// @param Collider class to get information about the gameObject the item collided with
	private void OnTriggerEnter(Collider other)
	{
		// Switching depending on the Colliders name
		int count = m_playerList.Count;

		if (!m_playerHolding)
		{
			if (other.tag == "Player")
			{
				Player holder = other.GetComponent<Player>();

				m_playerList.Insert(count, Timer.PlayerGet(holder.GetPlayerIndex()));
			}
		}

		// If there is more than one player in the playerList
		if (m_playerList.Count > 1)
		{
			// Loop through each player in the list
			for (int i = 0; i < m_playerList.Count - 1; i++)
			{
				// Remove any duplicated players
				if (m_playerList[i].name == m_playerList[i + 1].name)
				{
					m_playerList.RemoveAt(i);
				}
			}
		}

		// Check that there is data in the playerList
		if (m_playerList != null)
		{
			// Make sure the playerList is larger than zero
			if (m_playerList.Count > 0)
			{
				m_rigidbody.isKinematic = true;
				foreach (Collider collider in m_model)
				{
					collider.enabled = true;
				}
			}
		}

		// If the item is thrown
		if (m_isThrown)
		{
			// Check through each player
			foreach (Player player in m_playerList)
			{
				// And see if they are currently holding an item
				if (player.GetItem() != null)
				{
					// Set kinematic to false and turn off the colliders on the model
					// Allows for the item to collide with the player and bounce off of them
					m_rigidbody.isKinematic = false;
					foreach (Collider Collider in m_model)
					{
						Collider.enabled = true;
					}
				}
				else// Otherwise the player isn't holding an item
				{
					// Also the player isn't the last player to be holding the item
					if (m_lastPlayerHolding != player)
					{
						// Set the new player holding to the item catcher
						m_playerHolding = player;

						// So pick up the item
						player.PickUpItem(this);

						// Set the new last player holding to current holder
						m_lastPlayerHolding = m_playerHolding;
					}
				}
			}
		}
		else// Otherwise the item isn't thrown
		{
			// And for every player in our list, turn on kinematic and set collisions to true
			// Keeps the item from falling through the world and also allows player to walk through them
			foreach (Player player in m_playerList)
			{
				foreach (Collider Collider in m_model)
				{
					Collider.enabled = false;
				}
			}
		}
	}

	// @brief Checks the collision that has occured and reacts accordingly whilst the item
	//		  is thrown. If a collision has taken place the normal of the contact point is
	//		  calculated, force is then applied to the object based on that normal to ricochet
	//		  it away
	// @param Collision class to get information about the gameObject the item collided with
	private void OnCollisionEnter(Collision collision)
	{
		// If the item is thrown
		if (m_isThrown /*|| m_isDropped*/)
		{
			// Allow for gravity to affect the item
			m_rigidbody.useGravity = true;

			// Calculate the normal and apply it to the force of the item multipled by a scalar
			//if (!m_isDropped)
			{
				Vector3 normal = collision.contacts[0].normal;
				m_rigidbody.AddForce(normal * m_bounceForce, ForceMode.Impulse);
			}

			// Make sure the item is no longer thrown
			m_isThrown = false;
			m_isDropped = false;
            if (m_losesChargesOnThrow)
            {
                m_charges = 0;
            }
        }
	}

	// @brief Checks when another Collider has exited the trigger area
	//		  Depending on the name of the Collider, we can tell what
	//		  player is out of range of the item
	// @param Collider class to get information about the gameObject the item collided with
	private void OnTriggerExit(Collider other)
	{
		// Re-enable all the colliders in the model
		foreach (Collider collider in m_model)
		{
			collider.enabled = true;
		}

		// Check that there has been at least one interaction with the item
		if (m_lastPlayerHolding != null)
		{
			// If so, ignore the collisions with that last player
			// This is done keep items from balancing on the head of the player
			for (int i = 0; i < this.m_model.Count; i++)
			{
				Physics.IgnoreCollision(m_model[i], m_lastPlayerHolding.GetComponent<Collider>(), true);
			}
		}

		// Remove all positional and rotational constrations on the item
		m_rigidbody.constraints = RigidbodyConstraints.None;

		if (other.tag == "Player")
		{
			Player holder = other.GetComponent<Player>();

			m_playerList.Remove(Timer.PlayerGet(holder.GetPlayerIndex()));
		}

		// If there is no player in the list
		if (m_playerList.Count == 0)
		{
			// Item will not fall through world or cause player to walk on it
			m_rigidbody.isKinematic = false;
			foreach (Collider Collider in m_model)
			{
				Collider.enabled = true;
			}
		}
	}
}