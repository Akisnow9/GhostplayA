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
	/* Variables that store UI base data */
	[Header("Drag each panel within 'Joining' to this list")]
	[SerializeField] private List<GameObject> m_joinPanel;		 // Store the images that will be displayed by default
	[Header("Drag the UI that will appear once a player joins here")]
	[SerializeField] private GameObject m_confirmUI;			 // Store the confirmation text as a game object

	/* Variables that store Player base data */
	[Header("Drag each player within 'Characters' to this list")]
	[SerializeField] private List<GameObject> m_players;		 // Store the gameObjects that will display the player once pressed
	[Header("Drag each player HatAttachPoint to this list")]
	[SerializeField] private List<GameObject> m_hatAttachPoints; // Store the location of each players 'hat' attach point
	private int m_playersAdded = 0;                              // Store how many players are added to the game

	/* Variables that store Hat base data */
	[Header("Drag each hat prefab to this list")]
	[SerializeField] private List<GameObject> m_hats;            // Store each hat prefab for the players to pick from

	/* Variables that require public but the designers shouldn't touch! */
	[Header("DO NOT TOUCH THESE LISTS")]
	public List<GameObject> m_listOfUI;
	public List<GameObject> m_currentHat;

	//*************************************************************************************
	// Base class functionality
	//*************************************************************************************

	private void Start()
	{
		//for (int i = 0; i < Menus.GetMaxNumOfPlayers(); i++)
		//{
		//	m_listOfUI.Insert(i, Instantiate(m_confirmUI));
		//	Vector3 uiOffset = new Vector3(0, 0.2f, 0);
		//	Vector3 uiLocation = m_players[i].transform.position - uiOffset;
		//	m_listOfUI[i].transform.position = uiLocation;
		//	m_listOfUI[i].SetActive(false);
		//}
	}


	// @brief Update is called once per frame
	//		  Handles adding players to the game and starting the game
	private void Update()
	{
		// Add players to the game
		//AddPlayer();

		//HatSelection();

		//ReadyUp();

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

		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables holder = Menus.GetPlayerInformation(i);
			ControllerVibrato(holder);
		}
	}

	private void AddPlayer()
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
				if (XCI.GetButtonDown(XboxButton.A, controller))
				{
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
						newPlayer.Player = m_playersAdded;
						//newPlayer.HatID = m_playersAdded;
						Menus.AddToPlayerList(newPlayer, m_playersAdded);

						// Enable the panel to occupy the index position by a controller
						m_players[m_playersAdded].SetActive(true);

						// Disable the 'Joining' panel
						m_joinPanel[m_playersAdded].SetActive(false);

						// Display some dope hats
						//m_currentHat.Insert(m_playersAdded, Instantiate(m_hats[m_playersAdded]));
						//m_currentHat[m_playersAdded].transform.position = m_hatAttachPoints[m_playersAdded].transform.position;
						//m_currentHat[m_playersAdded].transform.forward = m_hatAttachPoints[m_playersAdded].transform.forward;
						//m_currentHat[m_playersAdded].SetActive(true);

						// Create the confirm panel
						m_listOfUI[m_playersAdded].SetActive(true);

						// Increase the amount of players added
						m_playersAdded++;
					}
					else // Handle the last controller plugged in
					{
						// Add the new player to the playerList
						StaticVariables newPlayer = new StaticVariables();
						newPlayer.Controller = controller;
						newPlayer.Player = m_playersAdded;
						//newPlayer.HatID = m_playersAdded;
						Menus.AddToPlayerList(newPlayer, m_playersAdded);

						// Enable the panel to occupy the index position by a controller
						m_players[m_playersAdded].SetActive(true);

						// Disable the 'Joining' panel
						m_joinPanel[m_playersAdded].SetActive(false);

						// Display some dope hats
						//m_currentHat.Insert(m_playersAdded, Instantiate(m_hats[m_playersAdded]));
						//m_currentHat[m_playersAdded].transform.position = m_hatAttachPoints[m_playersAdded].transform.position;
						//m_currentHat[m_playersAdded].transform.forward = m_hatAttachPoints[m_playersAdded].transform.forward;
						//m_currentHat[m_playersAdded].SetActive(true);

						// Create the confirm panel
						m_listOfUI[m_playersAdded].SetActive(true);

						// Increase the amount of players added
						m_playersAdded++;
					}
				}
			}
		}
	}

	private void HatSelection()
	{
		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables holder = Menus.GetPlayerInformation(i);
			XboxController controller = holder.Controller;

			if (XCI.GetButtonDown(XboxButton.DPadRight, controller))
			{
				//Destroy(m_currentHat[holder.HatID]);

				//if ((holder.HatID + 1) > m_hats.Count)
				//{
				//	holder.HatID = 0;
				//}

				//m_currentHat.Insert(m_currentHat.Count, Instantiate(m_hats[holder.HatID++]));
				//m_currentHat[holder.HatID].transform.position = m_hatAttachPoints[i].transform.position;
				//m_currentHat[holder.HatID].transform.forward = m_hatAttachPoints[i].transform.forward;
				//m_currentHat[holder.HatID].SetActive(true);
				//holder.HatID += 1;
			}
		}
	}

	private void ReadyUp()
	{
		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables holder = Menus.GetPlayerInformation(i);
			XboxController controller = holder.Controller;

			if (XCI.GetButtonDown(XboxButton.Y, controller))
			{
				holder.IsReady = true;
				m_listOfUI[i].GetComponent<ConfirmController>().ChangeConfirmState(holder.IsReady);
			}
		}
	}

	private void ControllerVibrato(StaticVariables a_holder)
	{
		if ((a_holder.TimeToStopVibrato * 30) > 0)
		{
			XCI.SetVibration(a_holder.Controller, 1f, 1f);
			a_holder.TimeToStopVibrato -= Time.time;
		}
		else
		{
			XCI.StopVibration(a_holder.Controller);
		}
	}
}