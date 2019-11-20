/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Controls adding players to the game
 * Data Created: 9th Sep, 2019
 * Last Modified: 11th Nov, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

// @detail The PlayerAdd class is in charge of handling adding players to the game
//		   We decided on changing how we use XCI. Instead of hard coding each
//		   game object to a certain controller, we can instead allow the players to join in
//		   at any index. Each time a player do+es join the game, that player position will
//		   become occupied. This ensures no two players can control one game object. Finally
//		   we restrict the players abilities to continous take up positions by checking if
//		   there is any data that already stores their controller information.
public class PlayerAdd : MonoBehaviour
{
	/*** Variables that store UI base data ***/
	/*****************************************/
	[Header("Drag each panel within 'Joining' to this list")]
	[SerializeField] private List<GameObject> m_joinPanel;		    // Store the images that will be displayed by default

	[Header("Drag the UI that will appear once a player joins here")]
	[SerializeField] private GameObject m_confirmUI;                // Store the confirmation text as a game object

	/*** Variables that store Player base data ***/
	/*********************************************/
	[Header("Drag each material to this list")]
	[SerializeField] private List<Material> m_playerMats;			// Store the different player colours
    [SerializeField] private List<Material> m_hatMaterials;         // Store the different player hats
	[Header("Drag each unready player, attach point, and shirt within 'Characters' to this list")]
	[SerializeField] private List<GameObject> m_unreadyPlayers;	    // Store the gameObjects that will display the player once pressed
	[SerializeField] private List<Transform> m_unreadyAttachPoints; // Store the location of each players 'hat' attach point
	[SerializeField] private List<GameObject> m_unreadyShirt;		// Store the location of each players shirt

	[Header("Drag each ready player, attach point, and shirt within 'CharactersReady' to this list")]
	[SerializeField] private List<GameObject> m_readyPlayers;       // Store the gameObjects that will display the player once pressed
	[SerializeField] private List<Transform> m_readyAttachPoints;   // Store the location of each players 'hat' attach point
	[SerializeField] private List<GameObject> m_readyShirt;			// Store the location of each players shirt

	private int m_playersAdded = 0;                                 // Store how many players are added to the game

	/*** Variables that store Hat base data ***/
	/******************************************/
	[Header("Drag each hat prefab to this list")]
	[SerializeField] private List<GameObject> m_hats;               // Store each hat prefab for the players to pick from
	private GameObject m_currentHat;
	private bool m_wasChanged;										// Store if the hat was changed last update frame
    [SerializeField] private float m_delayBetweentHatChanges = 1.0f;
	/*** Other required variables ***/
	/************************************************************************/
	[HideInInspector] public List<GameObject> m_listOfUI;
    private float m_nextChange;
    //*************************************************************************************
    // Base class functionality
    //*************************************************************************************

    private void Start()
	{
		for (int i = 0; i < Menus.GetMaxNumOfPlayers(); i++)
		{
			m_listOfUI.Insert(i, Instantiate(m_confirmUI));
			Vector3 uiOffset = new Vector3(0, 0.2f, 0);
			Vector3 uiLocation = m_unreadyPlayers[i].transform.position - uiOffset;
			m_listOfUI[i].transform.position = uiLocation;
			m_listOfUI[i].SetActive(false);
		}

		m_wasChanged = false;
	}


	// @brief Update is called once per frame
	//		  Handles adding players to the game and starting the game
	private void Update()
	{
		// Add players to the game
		AddPlayer();

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

				// Only let the player change hat and ready up if they have not already readied up
				if (!holder.IsReady)
				{
					HatSelection();

					ReadyUp();
				}
				else
				{
					// Check for the players input
					if (XCI.GetButtonDown(XboxButton.Start, controller))
					{
						Menus.StartGame();
					}
				}

			}
		}

		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables holder = Menus.GetPlayerInformation(i);
			ControllerVibrato(holder);
		}
	}

	private void LateUpdate()
	{
		m_wasChanged = false;
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
						newPlayer.Hat = m_hats[(int)e_Hats.NONE];
						newPlayer.HatID = (e_Hats.NONE);
						newPlayer.PlayerMaterial = m_playerMats[m_playersAdded];
						Menus.AddToPlayerList(newPlayer, m_playersAdded);

						// Enable the panel to occupy the index position by a controller
						m_unreadyPlayers[m_playersAdded].SetActive(true);

						// Disable the 'Joining' panel
						m_joinPanel[m_playersAdded].SetActive(false);

						// Display some dope hats
						newPlayer.Hat = Instantiate(m_hats[(int)e_Hats.NONE]);
						newPlayer.Hat.transform.SetParent(m_unreadyAttachPoints[m_playersAdded]);
						newPlayer.Hat.transform.ResetTransform();

						// Create the confirm panel
						m_listOfUI[m_playersAdded].SetActive(true);

						// Change the shirt colour of the player
						Material newMat = Instantiate(newPlayer.PlayerMaterial);
						newMat.SetColor(newPlayer.PlayerMaterial.name, newPlayer.PlayerMaterial.color);

						SkinnedMeshRenderer thisRenderer = m_unreadyShirt[m_playersAdded].GetComponent<SkinnedMeshRenderer>();
						thisRenderer.material = newMat;

						// Increase the amount of players added
						m_playersAdded++;
					}
					else // Handle the last controller plugged in
					{
						// Add the new player to the playerList
						StaticVariables newPlayer = new StaticVariables();
						newPlayer.Controller = controller;
						newPlayer.Player = m_playersAdded;
						newPlayer.Hat = m_hats[((int)e_Hats.NONE)];
						newPlayer.HatID = (e_Hats.NONE);
                        //if(m_playersAdded == 0 && )

                        //Need to look at colour assaignment here.
						newPlayer.PlayerMaterial = m_playerMats[m_playersAdded];


                        //if (m_playersAdded == 0)// Means it's blue. 0123
                        //{
                        //    newPlayer.HatMaterial = m_hatMaterials[((int)newPlayer.HatID)];
                        //}
                        //else
                        //{
                        //    newPlayer.HatMaterial = m_hatMaterials[(m_playersAdded * 4) + (int)newPlayer.HatID]; // Do cool math here
                        //}
                        Menus.AddToPlayerList(newPlayer, m_playersAdded);

						// Enable the panel to occupy the index position by a controller
						m_unreadyPlayers[m_playersAdded].SetActive(true);

						// Disable the 'Joining' panel
						m_joinPanel[m_playersAdded].SetActive(false);

						// Display some dope hats
						newPlayer.Hat = Instantiate(m_hats[(int)newPlayer.HatID]);
						newPlayer.Hat.transform.SetParent(m_unreadyAttachPoints[m_playersAdded]);
						newPlayer.Hat.transform.ResetTransform();

						// Create the confirm panel
						m_listOfUI[m_playersAdded].SetActive(true);

						// Change the shirt colour of the player
						Material newMat = Instantiate(newPlayer.PlayerMaterial);
						newMat.SetColor(newPlayer.PlayerMaterial.name, newPlayer.PlayerMaterial.color);

                        SkinnedMeshRenderer thisRenderer = m_unreadyShirt[newPlayer.Player].GetComponent<SkinnedMeshRenderer>();
                        thisRenderer.material = newMat;

                        //Material newHat = Instantiate(newPlayer.HatMaterial);
                        //newHat.SetColor(newPlayer.HatMaterial.name, newPlayer.HatMaterial.color);

                        //MeshRenderer hatRenderer = newPlayer.Hat.GetComponent<Reference>().m_reference.GetComponent<MeshRenderer>();
                        //hatRenderer.material = newHat;

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

			// Don't allow a ready player to change hat
			if (holder.IsReady)
			{
				continue; // Continue the loop but skip this iteration
			}

			// Dont allow 'double' updates... Skipping over hats
			if (Time.time > m_nextChange) // Removed !m_wasChanged &&
            {

                float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controller);
                if (axisX > 0)
                {
                    if (holder.Hat != null)
                    {
                        Destroy(holder.Hat);
                    }
                    m_nextChange = Time.time + m_delayBetweentHatChanges;
                    ApplyModificationUp(holder);
                    break;
                }
                else if (axisX < 0)
                {
                    if (holder.Hat != null)
                    {
                        Destroy(holder.Hat);
                    }
                    m_nextChange = Time.time + m_delayBetweentHatChanges;
                    ApplyModificationDown(holder);
                    break;
                }
            }
        }
	}

	private void ApplyModificationUp(StaticVariables a_holder)
	{
		if (((int)a_holder.HatID) < m_hats.Count - 1)
		{
			a_holder.HatID++;
		}
		else
		{
			a_holder.HatID = e_Hats.NONE;
		}

		// Have to loop through all the other players to not allow two players to have the same hat
		// Each player requires a different silhouette for disability friendly gameplay
		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables otherPlayers = Menus.GetPlayerInformation(i);

			// Only have this affect players who have not readied-up
			//if (otherPlayers.IsReady)
			//{
			//	// Make sure we are not checking the player who is adjusting the hat to themself, only to other players
			//	if (otherPlayers.Player != a_holder.Player)
			//	{
			//		// Check if they are wearing the same hat
			//		if ((int)otherPlayers.HatID == (int)a_holder.HatID)
			//		{
			//			if (((int)a_holder.HatID) < m_hats.Count - 1)
			//			{
			//				a_holder.HatID++;
			//			}
			//			else
			//			{
			//				a_holder.HatID = e_Hats.NONE;
			//			}
			//		}
			//	}
			//}
		}

		a_holder.Hat = Instantiate(m_hats[(int)a_holder.HatID]);
		a_holder.Hat.transform.SetParent(m_unreadyAttachPoints[a_holder.Player]);
		a_holder.Hat.transform.ResetTransform();

        a_holder.HatMaterial = GetNewHatMaterial(a_holder.Player, a_holder.HatID);
        if (a_holder.HatMaterial != null)
        {
            a_holder.Hat.GetComponent<Reference>().m_reference.GetComponent<MeshRenderer>().material = a_holder.HatMaterial;
        }
        m_wasChanged = true;
	}

	private void ApplyModificationDown(StaticVariables a_holder)
	{
		if (((int)a_holder.HatID) > 0)
		{
			a_holder.HatID--;
		}
		else
		{
			a_holder.HatID = e_Hats.PAPERBOAT;
		}

		// Have to loop through all the other players to not allow two players to have the same hat
		// Each player requires a different silhouette for disability friendly gameplay
		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables otherPlayers = Menus.GetPlayerInformation(i);

			// Only have this affect players who have not readied-up
			//if (otherPlayers.IsReady)
			//{
			//	// Make sure we are not checking the player who is adjusting the hat to themself, only to other players
			//	if (otherPlayers.Player != a_holder.Player)
			//	{
			//		// Check if they are wearing the same hat
			//		if ((int)otherPlayers.HatID == (int)a_holder.HatID)
			//		{
			//			if (((int)a_holder.HatID) > 0)
			//			{
			//				a_holder.HatID--;
			//			}
			//			else
			//			{
			//				a_holder.HatID = e_Hats.PAPERBOAT;
			//			}
			//		}
			//	}
			//}
		}

		a_holder.Hat = Instantiate(m_hats[(int)a_holder.HatID]);
		a_holder.Hat.transform.SetParent(m_unreadyAttachPoints[a_holder.Player]);
		a_holder.Hat.transform.ResetTransform();
        // Gets the material
        a_holder.HatMaterial = GetNewHatMaterial(a_holder.Player, a_holder.HatID);
        // Material needs to be applied to model
        if (a_holder.HatMaterial != null)
        {
            a_holder.Hat.GetComponent<Reference>().m_reference.GetComponent<MeshRenderer>().material = a_holder.HatMaterial;
        }
        m_wasChanged = true;
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
				LockedIn(holder);
			}
		}
	}

    private void LockedIn(StaticVariables a_holder)
    {
        // Enable the readied-up playe model
        m_unreadyPlayers[a_holder.Player].SetActive(false);
        m_readyPlayers[a_holder.Player].SetActive(true);

        // Set the hat to the new model
        a_holder.Hat.transform.SetParent(m_readyAttachPoints[a_holder.Player]);
        a_holder.Hat.transform.ResetTransform();

        // Set the material
        Material newMat = Instantiate(a_holder.PlayerMaterial);
        newMat.SetColor(a_holder.PlayerMaterial.name, a_holder.PlayerMaterial.color);

        if (a_holder.HatID != e_Hats.NONE)
        {
            a_holder.Hat.GetComponent<Reference>().m_reference.GetComponent<MeshRenderer>().material = a_holder.HatMaterial;
        }
        SkinnedMeshRenderer thisRenderer = m_readyShirt[a_holder.Player].GetComponent<SkinnedMeshRenderer>();
		thisRenderer.material = newMat;
	}

	private void ControllerVibrato(StaticVariables a_holder)
	{
		//if ((a_holder.TimeToStopVibrato * 30) > 0)
		//{
		//	XCI.SetVibration(a_holder.Controller, 1f, 1f);
		//	a_holder.TimeToStopVibrato -= Time.time;
		//}
		//else
		//{
		//	XCI.StopVibration(a_holder.Controller);
		//}
	}

    private Material GetNewHatMaterial(int a_playerNumber, e_Hats a_hatId)
    {
        if(a_playerNumber == 0)
        {
            if (a_hatId != e_Hats.NONE)
                return m_hatMaterials[(int)a_hatId - 1];
            else
                return null;
        }
        else
        {
            if (a_hatId != e_Hats.NONE)
                return m_hatMaterials[a_playerNumber * 4 + (int)a_hatId - 1];
            else
                return null;
        }
    }

}