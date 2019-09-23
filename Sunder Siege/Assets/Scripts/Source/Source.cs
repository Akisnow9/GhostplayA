/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: The item spawners and item refill sources
 * Data Created: 18th Sep, 2019
 * Last Modified: 21st Sep, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Source : MonoBehaviour
{
    [SerializeField] private bool m_spawnsItem; // Does this spawn items or does it refill items.
    [SerializeField] private Item m_itemToSpawn; // If it spawns items which item? Will need to drag an item from an off screen version.

    private List<Item> m_spawnedItems; // Keeps track of spawned items so they can be despawnd if they're not attached to a player for a period of time.
    // -- needs a variable to pass in on creation for how long they will last out of a players range. Like if m_playerlist == 0 || m_playerlist== null count down.

    [SerializeField] private E_Quality m_willRefill; // This is what the object give 'charges' back to if it gives charges. Or what item it spawns.
    public List<Player> m_playerList; // Store the different player in range



    private void Update()
    {
        if (m_playerList != null)
            if (m_playerList.Count > 0)
            {
                foreach (Player player in m_playerList)
                {
                    Item playerItem = player.GetItem();
                    XboxController controller = player.controller;
                    // Item playerItem = player.GetItem() Get palyer item here then use it for the checks below and increase charges ect
                    if (XCI.GetButtonDown(XboxButton.A, controller))
                    {
                        if (playerItem.GetCharges() < 3 && playerItem.IsRefillable())
                        {
                            playerItem.SwingTrue();
                            playerItem.RefillCharges();
                        }
                        else if(m_spawnsItem)
                        {
                            //m_itemToSpawn; // Need to add items to list if creation is possible. Might be better to use an object pool and just choose an inactive one and move it here. Will allow hard limits on amount of items.
                        }
                        else
                        {
                            playerItem.SwingFalse();
                        }
                    }
                   
                }
            }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Switching depending on the colliders name
        int count = m_playerList.Count;
        switch (other.name)
        {
            // Each case is the players triggerbox string
            case "Player1":
                m_playerList.Insert(count, Timer.PlayerGet(0)); // Add the correct player to the playerList
                break;
            case "Player2":
                m_playerList.Insert(count, Timer.PlayerGet(1));
                break;
            case "Player3":
                m_playerList.Insert(count, Timer.PlayerGet(2));
                break;
            case "Player4":
                m_playerList.Insert(count, Timer.PlayerGet(3));
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Switching depending on the colliders name
        switch (other.name)
        {
            // Each case is the players triggerbox string
            case "Player1":
                m_playerList.Remove(Timer.PlayerGet(0)); // remove the correct player from the playerList
                break;
            case "Player2":
                m_playerList.Remove(Timer.PlayerGet(1));
                break;
            case "Player3":
                m_playerList.Remove(Timer.PlayerGet(2));
                break;
            case "Player4":
                m_playerList.Remove(Timer.PlayerGet(3));
                break;
            default:
                break;
        }
    }
}
