/***************************************************
 * Written By: Anton Huber, Eric Brkic
 * Purpose: To keep gametime and trigger events
 * Data Created: 14th Sep, 2019
 * Last Modified: 14th Oct, 2019
**************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    //*************************************************************************************
    // When events will occur. 
    // Eg: Once 'm_timeLimit' reaches 'm_eventTriggerTime' there will be 'm_numberOfEvents' 
    // over the next 'm_eventTriggerTime'
    //*************************************************************************************
    //** Player based info **//

    [SerializeField] private Player m_playerToSpawn;
    //[HideInInspector]
    public List<Player> m_playerList;
    public List<GameObject> m_spawnPoints;
    public List<Material> m_playerMats;
	public List<GameObject> m_hats;
    [SerializeField] private List<AudioMixerGroup> m_mixers;
    [SerializeField] private SoundManager m_soundManager;


    [SerializeField] private float m_timeLimit = 60; // The timer will count down from this point. Set from Unity Editor.
    [SerializeField] private float m_timeBeforeFail = 15; // Once a problem has activated how long before the player loses a life.
    [SerializeField] private int m_numberofLives = 5; // How many times items from the active list can expire.

    //[SerializeField] private float m_minRangeOfProblem = 3; // Minus this from max. Then add to repeat every. Only for problems.
    //[SerializeField] private float m_maxRangeOfProblem = 15; // This minus min. Then add to repeat every. Only for problems

    private float m_timeScale = 1; // This can be used to modify the rate of seconds passing also movement speed and animation. Useful if people want to include a slowdown effect but needs to be included in everything that is effected. Eg movement/ animation will need to be multiplied by this all the time.

    [HideInInspector]
    [SerializeField] private int m_numOfInactiveProblems; // The amount of problems in the list that are inactive. Easier then checking every inactive amounts in list.

    //[HideInInspector]
    public List<Events> m_pendingEventList; // The event list. Events are added from the editor.
    public List<Problem> m_problemList; // List of problems. Problems added to this list will be randomly selected.

    private static Timer instance = null;

    [SerializeField] private List<SoundRequester> m_sounds;

    [SerializeField] private bool m_keyboard = false;

    private void Awake()
    {
        instance = this;
   
		// This needs to be re-enabled
		Player playerClone;
		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			// Create the players an attach their controllers
			playerClone = Instantiate(m_playerToSpawn);
            m_playerList.Insert(i, playerClone);
			StaticVariables holder = Menus.GetPlayerInformation(i);
			playerClone.controller = holder.Controller;

			// Set the players spawn point
			playerClone.transform.position = m_spawnPoints[i].transform.position;

			// Set the players index
			playerClone.SetPlayerIndex(holder.Player);

            // Set player details
            if (holder.HatID != e_Hats.NONE)
            {
                playerClone.PlayerHat = m_hats[(int)holder.HatID];
                playerClone.HatID = holder.HatID;

                // Creates player hat

                playerClone.PlayerHat = Instantiate(Timer.Hat[(int)playerClone.HatID]);
                playerClone.PlayerHat.transform.SetParent(playerClone.HatattachPoint.transform);
                playerClone.PlayerHat.transform.ResetTransform();

                Material newMat = Instantiate(holder.HatMaterial);
                newMat.SetColor(holder.HatMaterial.name, holder.HatMaterial.color);

                GameObject m = playerClone.PlayerHat.GetComponent<Reference>().m_reference;
                MeshRenderer a = m.GetComponent<MeshRenderer>();
                //MeshRenderer hatRenderer = m_playerList[i].PlayerHat.GetComponent<Reference>().gameObject.GetComponent<MeshRenderer>();
                a.material = newMat;
            }
			// Add the players to the playerList

			// Set the shirts material
			Material newMaterial = Instantiate(holder.PlayerMaterial);
			newMaterial.SetColor(holder.PlayerMaterial.name, holder.PlayerMaterial.color);

			SkinnedMeshRenderer thisRenderer = m_playerList[i].GetPlayerShirt().GetComponent<SkinnedMeshRenderer>();
			thisRenderer.material = newMaterial;
		}
    }
    //GameObject newObject = Instantiate();
    //GameObject.AddComponent<Events>();

    // Start is called before the first frame update
    void Start()
    {
        // THIS IS WHERE I AM BRo!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //int maxNumberOfPlayers = 4; // Should be passed from the menu
        //for (int i = maxNumberOfPlayers; i < Timer.PlayerAmountGet(); i--)
        //{
        //    string enm = "TriggerProblem" + i + "PlayerUp";
        //    E_Event eve = (E_Event)E_Event.Parse(typeof(E_Event), enm, true);

        //    m_pendingEventList.RemoveAll(eve);
        //            // Need to remove events labeld TriggerProblem"i"PlayerUp
             
            
        //}
        if (m_sounds != null)
            if (m_sounds.Count != 0)
            {
                foreach (SoundRequester sounds in m_sounds)
                {
                    sounds.SetupSound(this.gameObject);
                }
            }
        m_numOfInactiveProblems = m_problemList.Count;
        //Needs safety here.
        Timer.SoundMangerGet().Play(m_sounds, WhenToPlaySound.LevelStart);
    }

    // Update is called once per frame
    void Update()
    {
        CheckEventsLists();
        if (CheckProblems()) // Returns true if fail state reached. Either timer hit 0 or lives hit 0.
        {
            if (m_numberofLives <= 0)
                SceneManager.LoadScene(3);
            else
                SceneManager.LoadScene(2);
        }
        m_timeLimit -= m_timeScale * Time.deltaTime; // minus the time between frames * the scaler.
        if (m_sounds.Count != 0)
            Timer.SoundMangerGet().CheckAudio(m_sounds);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }



    public static bool GetKeyBoard()
    {
        return instance.m_keyboard;
    }



    public static int PlayerAmountGet()
    {
        return instance.m_playerList.Count;
    }


    public static Player PlayerGet(int a_index)
    {
        return instance.m_playerList[a_index];
    }

	public static List<GameObject> Hat
	{
		get
		{
			//if (instance.m_hats != null)
			{
				return instance.m_hats;
			}
		}
	}

    public static SoundManager SoundMangerGet()
    {
        return instance.m_soundManager;
    }

    //Gets time for ui
    public static float TimeGet()
    {
        return instance.m_timeLimit;
    }
    
    public static int LivesGet()
    {
        return instance.m_numberofLives;
    }

    private void CheckEventsLists()// Not actual name. Might become CheckState.
    {
        // @In charge of queing up and starting new problems. 
        if ((int)m_timeLimit % 1 == 0) // Checks every second rather then every frame. // Might have issues since the time passing may skip the 1 second mark.
        {
            int amountOfEvents = m_pendingEventList.Count;
            for (int i = 0; i < amountOfEvents; i++)
            {
                if (m_pendingEventList[i] == null) // A safety check to make sure the list isn't full of nothing. Will get stuck in an endless loop otherwise.
                {
                    m_pendingEventList.Remove(m_pendingEventList[i]); // Remove blank events.
                    --amountOfEvents;
                    --i;
                }
                if (m_pendingEventList[i].m_eventTriggerTime >= m_timeLimit)  // Needs to check what data is contained on the list - The time needs to be checked first.
                {
                    switch (m_pendingEventList[i].m_event)
                    {
                        case E_Event.SetNumberOfLives:
                            SetNumberOfLives(m_pendingEventList[i].m_dataToModify);
                            break;
                        case E_Event.SetTimeBeforeFail:
                            SetTimeBeforeFail(m_pendingEventList[i].m_dataToModify);
                            break;
                        case E_Event.TriggerProblem1PlayerUp:
                        case E_Event.TriggerProblem2PlayerUp:
                        case E_Event.TriggerProblem3PlayerUp:
                        case E_Event.TriggerProblem4PlayerUp:
                            TriggerProblem(m_pendingEventList[i]);
                            break;
                    }
                    if (!m_pendingEventList[i].m_repeatable)
                    {
                        m_pendingEventList.Remove(m_pendingEventList[i]); // Changes the event time.
                        --amountOfEvents;
                        --i;
                    }
                    else
                        EventAdjust(m_pendingEventList[i]);
                }
            }
        }
    }


	bool CheckProblems()
	{
        // The function will handle all the swapping of states. Will return true if something on the active list 
        // has expired. Need to deduct lives here only. Version that uses a List of problems.
        // It will also check to see if a problem has completed its 'pending' animation

        for (int i = 0; i < m_problemList.Count; i++)
        //foreach (Problem problem in m_problemList) -- Due to the way i'm modifying the 'm_problemlist' I can't use for each loops. It seems to get information about the loop at some other point rather then say the count in the above for loop.
        {
            if (m_problemList[i].CheckActive(m_timeLimit) && !m_problemList[i].GetPending()) // Checks if a problem has expired. -- Can probably be used more efficently now that list sorts active
            {
                m_problemList[i].Deactivate(); 
                m_numberofLives--; // Subtract a life.
                Problem addToFront = m_problemList[i]; // Copies itself. -- // Might need to be moved into a function. That can be called elsewhere
                m_problemList.Remove(m_problemList[i]); // Removes itself. -- // Might need to be moved into a function. That can be called elsewhere
                // If permanent Get rid of insert and encapsulate with an if(maxbutton presses == current button presses) Then insert.
                // Consider changing color of model. New function perma break.
                m_problemList.Insert(0, addToFront); // Puts problem to the front of the list.    --    This will be skipped if the problem has a permanent broken state.  -- // Might need to be moved into a function. That can be called elsewhere
                m_numOfInactiveProblems++; // Might need to be moved into a function. That can be called elsewhere
            }
            else if (m_problemList[i].GetPending())                          // If active will now check pending status.
                if (m_problemList[i].CheckPending())
                {
                    m_problemList[i].Activate(m_timeLimit - m_timeBeforeFail);
                }
        }
		return (m_numberofLives <= 0 || m_timeLimit <= 0.0f);
	}



    //*************************************************************************************
    // Event handler
    // Will contain all funcitons for the switch statements. Line 99
    //*************************************************************************************

    private void EventAdjust(Events a_event)
    {
        // @Adjust the event in the queue placeing it in the event queue. Could probably make some of these variables private and have get and set functions.
        a_event.m_eventTriggerTime = m_timeLimit - a_event.m_repeatEvery + a_event.GetRange();
    }

    void TriggerProblem(Events a_events)
    {
        if (m_numOfInactiveProblems > 0)
        {
            int position = Random.Range(0, m_numOfInactiveProblems);
            m_numOfInactiveProblems--;                                                  // Variable keeping track of inactive problems is adjusted.
            m_problemList[position].Pending();
            // m_problemList[position].Activate(m_timeLimit - m_timeBeforeFail);           // May need to be changed to pending -- The rest just makes sure it is not rolled again.
            Problem addtoback = m_problemList[position];                                // Copies what is in the now activated location.
            m_problemList.Remove(m_problemList[position]);                              // Removes the original problem from the list.
            m_problemList.Insert(m_problemList.Count, addtoback);
        }                                                                               // Adds the copied data to the end of the list.
    }


    void SetNumberOfLives(int a_numOfLives)
    {
        m_numberofLives = a_numOfLives;
    }
    void SetTimeBeforeFail(float a_timeBeforeFail)
    {
        m_timeBeforeFail = a_timeBeforeFail;
    }

    public static void SolvedProblem()
    {
        // Once a player fixes a problem the number of inactive problems increases.
        instance.m_numOfInactiveProblems++;
    }

}










    //*************************************************************************************
    // This is a rough outline of a class that can handle difficulty changes.
    // The timer will contain a list of sorted events that will be checked if the time 
    // has been met. Then an enum which will be used to call the appropriate function 
    // and then number stored in the list will modify the current value.
    //
    // eg; At the time of '120' function associated with enum 'NumberOfEvents' will be called
    // and set the value with 'n' which will be passed in by a variable in the list.
    //
    // Each item on the list will contain these variables.
    // float m_eventTime // When function gets called.
    // E_ModifyValue m_modifyvalue // Calls funciton.
    // float m_newValue // What gets passed into the function to set it.
    //*************************************************************************************





    // Used to modify how many events occur over a period of time in game.
    // I'm considering creating a seperate class for it so that all the queue events can 
    // be stored on the same list. Will allow for a much easier time of modifying difficulty
    // during gameplay with events that can be changed easier with enums which will dictate 
    // what action will need to be taken.
    //void SetNumberOfEvents(int a_numberOfEvents)
    //{
    //    m_numberOfEvents = a_numberOfEvents;
    //}



