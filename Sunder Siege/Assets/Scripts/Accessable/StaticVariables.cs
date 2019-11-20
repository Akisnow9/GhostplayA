/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Store varibles needed globally
 * Data Created: 9th Sep, 2019
 * Last Modified: 13th Oct, 2019
 **************************************************/
using System.Collections.Generic;
using XboxCtrlrInput;
using UnityEngine;

// @brief Holds variables that are required throughout the runtime
//		  of the program.
public class StaticVariables
{
	private int player;				       // Store the player number to see what player it is in index
	private XboxController controller;     // Store the controller enum to see what controller is being used
	private float timeToStopVibrato = 15f; // Store how long the controller will vibrate
	private bool m_isReady = false;        // Store if the player has readied up or not
	private GameObject m_hat;			   // Store the actual hat game object
	private e_Hats m_hatID;                // Store the hats ID number to know which hat the player is wearing
	private Material m_playerMat;		   // Store the players colour
    private Material m_hatMat;             // Store the player hat material 


	//*************************************************************************************
	// Properties functionality
	//*************************************************************************************
	// @brief Properties for getting data in player
	public int Player
	{
		get { return player; }
		set { player = value; }
	}

	// @brief Properties for getting data in controller
	public XboxController Controller
	{
		get { return controller; }
		set { controller = value; }
	}

	// @brief Properties for getting vibration in controller
	public float TimeToStopVibrato
	{
		get { return timeToStopVibrato; }
		set { timeToStopVibrato = value; }
	}

	public bool IsReady
	{
		get { return m_isReady; }
		set { m_isReady = value; }
	}

	public GameObject Hat
	{
		get { return m_hat; }
		set { m_hat = value; }
	}

	public e_Hats HatID
	{
		get { return m_hatID; }
		set { m_hatID = value; }
	}

	public Material PlayerMaterial
	{
		get { return m_playerMat; }
		set { m_playerMat = value; }
	}

    public Material HatMaterial
    {
        get { return m_hatMat; }
        set { m_hatMat = value; }
    }

}
