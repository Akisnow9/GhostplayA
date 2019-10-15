/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Store varibles needed globally
 * Data Created: 9th Sep, 2019
 * Last Modified: 13th Oct, 2019
 **************************************************/
using System.Collections.Generic;
using XboxCtrlrInput;

// @brief Holds variables that are required throughout the runtime
//		  of the program.
public class StaticVariables
{
	private int player;				   // Store the player number to see what player it is in index
	private XboxController controller; // Store the controller enum to see what controller is being used

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
}
