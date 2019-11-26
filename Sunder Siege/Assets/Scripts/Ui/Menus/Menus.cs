/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: Handles moving between scenes and getting info on the globals
 * Data Created: 9th Sep, 2019
 * Last Modified: 13th Oct, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

// @detail The Menus class is handling switching between scenes when required depending
//		   on button presses, game logic, etc. The other information this class handles
//		   is the static global varaibles, as defined in StaticVariables.cs. Within the
//		   class we can access information within the StaticVariables.cs via a list.
//		   Using a static instance of the class we can recieve this info. We can also add
//		   to the information if we wish.
public class Menus : MonoBehaviour
{
    public List<StaticVariables> m_playerList;		   // Contains all the information that needs to be passed into the next scene
    [SerializeField] private int m_maxNumberOfPlayers; // If designers want to set fewer players. Can also be used as a scaler
													   // modify difficulty

	public GameObject m_MainMenu;					   // GameObject storing the Main Menu for deactiving/activating
    public GameObject m_creditsMenu;
	public GameObject m_PlayerAddMenu;                 // GameObject storing the Player Add Menu for deactiving/activating

	private static bool didQueryNumOfCtrlrs = false;   // Used for querying the number of controllers are plugged into the system

	public static Menus instance = null;               // Any static function needs to firstly use 'instance.' Then whatever to return over here


	//*************************************************************************************
	// Base class functionality
	//*************************************************************************************
	// @brief Runs at the start on the first call
	//		  Initialises the playerList and the instance
	public void Awake()
    {
		m_playerList = new List<StaticVariables>();
        instance = this; // Should now always be able to access m_playerList when using Menus.m_playerList.
    }

	// @brief Runs at the start of the games launch
	//		  Querys how many controllers are plugged into the system and outputs the amount
	private void Start()
	{
		if (!didQueryNumOfCtrlrs)
		{
			didQueryNumOfCtrlrs = true;

			int queriedNumOfCtrls = XCI.GetNumPluggedCtrlrs();
			if (queriedNumOfCtrls == 1)
			{
				Debug.Log("Only " + queriedNumOfCtrls + " Xbox Controller plugged in.");
			}
			else if (queriedNumOfCtrls == 0)
			{
				Debug.Log("No Xbox Controllers plugged in!");
			}
			else
			{
				Debug.Log(queriedNumOfCtrls + " Xbox Controllers plugged in.");
			}

			XCI.DEBUG_LogControllerNames();
		}
	}

	// @brief Update is called once per frame
	//		  Handles checking for controller input to go into the Player Add Menu
	private void Update()
	{
		// Allow the first controller to do the initial launch
		XboxController controller = XboxController.First;

		// Check for the correct input
		if (XCI.GetButtonDown(XboxButton.Start, controller))
		{
			// Deactivate the Main Menu, and activate the Player Add Menu
			m_MainMenu.SetActive(false);
			m_PlayerAddMenu.SetActive(true);
		}

		// If the player presses 'Back' the game will quit
		if (XCI.GetButtonDown(XboxButton.Back, controller))
		{
			QuitGame();
		}

        ReturnToMenu();
	}

	//*************************************************************************************
	// Menus core functionality
	//*************************************************************************************
	// @brief When called, the scene manager will load the next scene in the build index
	public static void StartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

    public void CreditsDisplay()
    {
        m_MainMenu.SetActive(false);
        m_creditsMenu.SetActive(true);
    }

    public void ReturnToMenu()
    {
        XboxController controller = XboxController.First;
        for (int i = 0; i < Menus.GetMaxNumOfPlayers(); i++)
        {
            if (XCI.GetButtonDown(XboxButton.Back, controller) || XCI.GetButtonDown(XboxButton.B, controller))
            {
                m_creditsMenu.SetActive(false);
                m_MainMenu.SetActive(true);
            }

            controller++;
        }

    }

	// @brief When called, the application will quit
	public void QuitGame()
	{
		// Debugs for in-engine use. You can't 'quit' the engine, only the build .exe
		Debug.Log("Quit");
		Application.Quit();
	}


	//*************************************************************************************
	// Static functionality
	//*************************************************************************************
	// @brief Gets the maximum amount of players allowed to be in game
	// @return Returns a int of the value for the maxNumberOfPlayers
	public static int GetMaxNumOfPlayers()
	{
		return instance.m_maxNumberOfPlayers;
	}

	// @brief Gets the amount of activated players in the player list
	// @return Returns a int of the value for the count of players in playerList
	public static int GetActivatedPlayerAmount()
    {
        return instance.m_playerList.Count; // Returns amount of players in list.
    }

	// @brief Gets the information of a specific player
	// @param Int data type to store the specifics players index being grabbed from
	// @return Returns the variables stored in StaticVariables from a certain player
	public static StaticVariables GetPlayerInformation(int a_player)
    {
		// Gets the player information which will be: what player, what controller, what hat
		// These are then assagined 
		if (instance.m_playerList[a_player] != null)
		{
			return instance.m_playerList[a_player]; // Returns staticVariable so will need to be stored then assaigned at awake of new scene
		}
		else
		{
            return null; // With a null return something has to be done by the caller to handle it. Should be a loop? or a for each? Exits on null
		}
    }

	// @brief Adds data to the player list
	// @param StaticVariable class which holds variables within that class
	// @param Int value to store what position the data will be stored
	// @return Returns the variables stored in StaticVariables from a certain player
	public static void AddToPlayerList(StaticVariables a_staticVariables, int a_postionToAdd)
    {
		// Make sure the data being passsed is valid
		if (a_staticVariables == null)
		{
			return;
		}
		else
		{
			// Will add data to list. Advised to use a for loop outside to ensure that you don't add more than m_maximumNumberOfPlayers
			instance.m_playerList.Insert(a_postionToAdd, a_staticVariables);
		}
    }
}
