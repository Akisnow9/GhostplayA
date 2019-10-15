/***************************************************
 * Written By: Eric Brkic, Ciaran Coppell
 * Purpose: Controls the Player Movement and Input
 * Data Created: 12th Sep, 2019
 * Last Modified: 16th Sep, 2019
 **************************************************/
using UnityEngine;
using XboxCtrlrInput;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float dashDistance = 10f;
	[SerializeField] private float dropOffset;
	public XboxController controller;
	//public Vector3 direction;
	//public Vector3 localDirection;

	private Item m_heldItem;    // The currently held item. 
	//private Vector3 newPosition;
	private Vector3 lastPosition;

    private Rigidbody rb;

	private static bool didQueryNumOfCtrlrs = false; // Why is this static?

	private int m_playerIndex;

	[SerializeField] private GameObject m_playerShirt;

	// Start is called before the first frame update
	void Start()
	{
		
		rb = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update()
	{
		Movement();
        if (m_heldItem != null)
            MoveItem();
	}

	void Movement()
	{
		// Check the players direction
		//direction = transform.position - lastPosition;
		//localDirection = transform.InverseTransformDirection(direction);


		// Remember the lastPosition the Player was
		lastPosition = transform.position;

		// Left stick movement
		//Read input
		//vec move = calculate offset based on input
		//transform.forward = move.normalised
		//pos = pos + move


		//newPosition = transform.position;
		float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controller);
		float axisY = XCI.GetAxis(XboxAxis.LeftStickY, controller);
		if (axisX != 0 || axisY != 0)
		{
			Vector3 move = new Vector3(axisX * movementSpeed * Time.deltaTime, 0, axisY * movementSpeed * Time.deltaTime);
			transform.forward = move.normalized;
            rb.AddForce(move, ForceMode.Impulse);
		}


		// 'X' dash movement
		if (XCI.GetButtonDown(XboxButton.X, controller))
		{
			if (dashDistance == 0 || dashDistance == 1)
			{
				return;
			}
			else
			{
				if (XCI.GetAxis(XboxAxis.LeftStickX, controller) != 0 || XCI.GetAxis(XboxAxis.LeftStickY, controller) != 0)
				{
					rb.AddForce(transform.forward * dashDistance, ForceMode.Impulse);
				}
				rb.velocity = Vector3.zero;
			}
		}
	}


    // Item Related Shenanigans


    private void MoveItem()
    {
        //If this works I will eat my fucking hat. -- It does.
        m_heldItem.transform.position = this.transform.position + m_heldItem.GetOffset();
        m_heldItem.transform.rotation = this.transform.rotation;
    }

    public void PickUpItem(Item a_item)
    {
        m_heldItem = a_item; // Now the player can be queired for what it is holding.
    }

    public void DropItem()
    {
        m_heldItem = null;
    }

	public void ThrowItem()
	{
		m_heldItem = null;
	}
    
    public Item GetItem()
    {
        return m_heldItem;
    }

	public int GetPlayerIndex()
	{
		return m_playerIndex;
	}

	public void SetPlayerIndex(int a_index)
	{
		m_playerIndex = a_index;
	}

	public GameObject GetPlayerShirt()
	{
		return m_playerShirt;
	}
}
