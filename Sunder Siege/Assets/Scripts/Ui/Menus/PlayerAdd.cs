/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Controls adding players to the game
 * Data Created: 9th Sep, 2019
 * Last Modified: 13th Oct, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

// @detail The PlayerAdd class is in charge of handling adding players to the game
//		   We decided on changing how we use XCI. Instead of hard coding each
//		   game object to a certain controller, we can instead allow the players to join in
//		   at any index. Each time a player does join the game, that player position will
//		   become occupied. This ensures no two players can control one game object. Finally
//		   we restrict the players abilities to continous take up positions by checking if
//		   there is any data that already stores their controller information.
public class PlayerAdd : MonoBehaviour
{
	[SerializeField] private List<GameObject> m_panels; // Store the images that will be displayed once a player is occupied
	private int m_playersAdded = 0;						// store how many players are added to the game
	private float m_pressRate = 2f;						// Store the rate of how many presses are allowed from a players controller
	private float m_pressDelay = 0.5f;                  // Store the delay done between presses

	//*************************************************************************************
	// Base class functionality
	//*************************************************************************************
	// @brief Update is called once per frame
	//		  Handles adding players to the game and starting the game
	private void Update()
	{
		// Loop through how many controllers are plugged in to the system
		for (int i = 0; i < XCI.GetNumPluggedCtrlrs(); i++)
		{
			// Check that we havent hit the max number of players allowed to play
			if (m_playersAdded + 1 <= Menus.GetMaxNumOfPlayers())
			{
				// Store the controller to the first enum plus the loops iteration
				XboxController controller = XboxController.First + i;

				// Check for A button press and that the program has gotten over the pressDelay
				if (XCI.GetButtonDown(XboxButton.A, controller) && Time.time >= m_pressDelay)
				{
					// Work out the new delay
					m_pressDelay = Time.time + 1f / m_pressRate;

					// Check for how many players are already activated
					for (int j = 0; j < Menus.GetActivatedPlayerAmount(); j++)
					{
						// Keep the same controller from joining the game multiple times
						StaticVariables holder = Menus.GetPlayerInformation(j);
						if (holder.Controller == controller)
						{
							Debug.Log("Controller Already Added.");
							return;
						}
					}

					// Check that the controller isn't already added
					if ((i + 1) < XCI.GetNumPluggedCtrlrs()) // If this works I'm going to slap my knee and yell Yee-Haw.
					{
						// Add the new player to the playerList
						StaticVariables newPlayer = new StaticVariables();
						newPlayer.Controller = controller;
						newPlayer.Player = i;
						// Attach a hat
						Menus.AddToPlayerList(newPlayer, m_playersAdded);

						// Enable the panel to occupy the index position by a controller
						m_panels[m_playersAdded].SetActive(true);

						// Increase the amount of players added
						m_playersAdded++;
					}
					else // Handle the last controller plugged in
					{
						// Add the new player to the playerList
						StaticVariables newPlayer = new StaticVariables();
						newPlayer.Controller = controller;
						newPlayer.Player = i;
						// Attach a hat
						Menus.AddToPlayerList(newPlayer, m_playersAdded);

						// Enable the panel to occupy the index position by a controller
						m_panels[m_playersAdded].SetActive(true);

						// Increase the amount of players added
						m_playersAdded++;
					}
				}
			}
		}

		// If there are no players in the list, the game cannot start
		if (Menus.GetActivatedPlayerAmount() <= 0)
		{
			return;
		}
		else
		{
			// Loop through each controller and check for an input to start the game
			for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
			{
				// Only allow players who have joined the game to start it
				StaticVariables holder = Menus.GetPlayerInformation(i);
				XboxController controller = holder.Controller;

				// Check for the players input
				if (XCI.GetButtonDown(XboxButton.Start, controller))
				{
					Menus.StartGame();
				}
			}
		}
		// Hat checking function here.!!!!!
	}

    // private void HatFunction() !!!!
    //{
    //  for loop checking each activated players associated input to see if they scrolled through hats.
    //  Change data(probably will be an enum) in static variable.
    // 
    //}
}   

