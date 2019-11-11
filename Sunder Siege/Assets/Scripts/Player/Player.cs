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
    private float dashStart;
    [SerializeField] private float dashCooldown;
    //[SerializeField] private float playerAcceleration = 10f;
    Vector3 m_startingPos;
    public XboxController controller;
    //public Vector3 direction;
    //public Vector3 localDirection;

    private Item m_heldItem;    // The currently held item. 
                                //private Vector3 newPosition;
    private Vector3 lastPosition;

    private Rigidbody rb;

    private static bool didQueryNumOfCtrlrs = false; // Why is this static?

    public int m_playerIndex;

    [SerializeField] private GameObject m_playerShirt;

    // Start is called before the first frame update
    void Start()
    {
        dashStart = Time.time;
        rb = GetComponent<Rigidbody>();
        m_startingPos = transform.position;
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

        //@brief Checks Axis of the Thumbstick of the XboxController
        //@param Vec 3, Vec 3
        //@return Direction of and rotation of player movement
        // implement an acceleration system with a speed cap
        if (Timer.GetKeyBoard()) // If true
        {
            float axisX = 0;
            float axisY = 0;
            // Get which direction from wsad.
            if (Input.GetKey(KeyCode.W))
            {
                axisY++;
            }
            if (Input.GetKey(KeyCode.S))
            {
                axisY--;
            }
            if (Input.GetKey(KeyCode.D))
            {
                axisX++;
            }
            if (Input.GetKey(KeyCode.A))
            {
                axisX--;
            }



            Vector3 move = new Vector3(axisX * movementSpeed * Time.deltaTime, 0, axisY * movementSpeed * Time.deltaTime);
            transform.forward = move.normalized; // normalises the move vec and turns it so that the X axis is facing the direction the player is moving
            rb.AddForce(move, ForceMode.Impulse); //creates a force pushing the player in the direction it's facing 
        }
        else
        {
            float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controller); // X axis of left thumbstick.
            float axisY = XCI.GetAxis(XboxAxis.LeftStickY, controller); // Y axis of left thumbstick.
            if (axisX != 0 || axisY != 0) // checks if there is an axis input.
            {
                Vector3 move = new Vector3(axisX * movementSpeed * Time.deltaTime, 0, axisY * movementSpeed * Time.deltaTime); // vec3 for movement direction and rotation
                transform.forward = move.normalized; // normalises the move vec and turns it so that the X axis is facing the direction the player is moving
                rb.AddForce(move, ForceMode.Impulse); //creates a force pushing the player in the direction it's facing 
            }
        }
        // implement a cooldown system for dashes
        // 'X' dash movement
        if (Time.time < dashStart + dashCooldown)
        {
            //Why?
        }
        else
        {
            if (Timer.GetKeyBoard())
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (dashDistance == 0 || dashDistance == 1) // if  dash has a distance of 0 or 1 then do nothing
                    {
                        return;
                    }
                    else
                    {
                        //if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                        {
                            rb.AddForce(transform.forward * dashDistance, ForceMode.Impulse); // applies a force to the player in the direction the player is facing
                            dashStart = Time.time;
                        }
                        rb.velocity = Vector3.zero; // the velocity is set to zero if the player dash is 0 or 1
                    }
                }

            }
            else
            {
                if (XCI.GetButtonDown(XboxButton.X, controller)) // if the X button is pressed then the player is applied a force
                {
                    if (dashDistance == 0 || dashDistance == 1) // if  dash has a distance of 0 or 1 then do nothing
                    {
                        return;
                    }
                    else
                    {
                        if (XCI.GetAxis(XboxAxis.LeftStickX, controller) != 0 || XCI.GetAxis(XboxAxis.LeftStickY, controller) != 0) // checks if x or y axis of left thumbstick has a value 
                        {
                            rb.AddForce(transform.forward * dashDistance, ForceMode.Impulse); // applies a force to the player in the direction the player is facing
                            dashStart = Time.time;
                        }
                        rb.velocity = Vector3.zero; // the velocity is set to zero if the player dash is 0 or 1
                    }
                }
            }
        }
    }





    // Item Related Shenanigans

        
    private void MoveItem()
    {
        //If this works I will eat my fucking hat. -- It does.
        m_heldItem.transform.position = this.transform.position + m_heldItem.GetOffset(); // transforms the items position with the players position above the players head
        m_heldItem.transform.rotation = this.transform.rotation; // follows the rotation of the player
    }

    public void PickUpItem(Item a_item)
    {
        m_heldItem = a_item; // Now the player can be queired for what it is holding.
    }

    public void DropItem()
    {
        m_heldItem = null; // "drops" the item allowing the player to pick up another by setting the held to null
    }

	public void ThrowItem()
	{
		m_heldItem = null; // "throws" the item and allows player to pick up a new item by setting to null
	}
    
    public Item GetItem()
    {
        return m_heldItem; // Checks what item is being held
    }

	public int GetPlayerIndex()
	{
		return m_playerIndex; //gets the XCI player index
	}

	public void SetPlayerIndex(int a_index)
	{
		m_playerIndex = a_index; //sets the XCI player index
	}

	public GameObject GetPlayerShirt()
	{
		return m_playerShirt; // Gets the shirt colour of the player
	}
    public void playerPosReset() // resets player position  
    {
        transform.position = m_startingPos;
        rb.velocity = Vector3.zero;
    }

}
