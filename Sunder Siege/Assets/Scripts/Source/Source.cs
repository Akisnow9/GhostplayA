/***************************************************
 * Written By: Eric Brkic, Anton Huber
 * Purpose: The item spawners and item refill sources
 * Data Created: 18th Sep, 2019
 * Last Modified: 14th Oct, 2019
 **************************************************/
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Source : MonoBehaviour
{
    public List<Player> m_playerList; // Store the different player in range
    public List<Item> m_spawnedItemList; // So the source can keep track of how many items it has spawned and cap it at a limmit
    [SerializeField] private E_Quality m_willRefill; // This is what the object give 'charges' back to if it gives charges. Or what item it spawns.

    // -- needs a variable to pass in on creation for how long they will last out of a players range. Like if m_playerlist == 0 || m_playerlist== null count down.

    [SerializeField] private bool m_spawnsItem; // Does this spawn items or does it refill items.
    [SerializeField] private Item m_itemToSpawn; // If it spawns items which item? Will need to drag an item from an off screen version.
    [SerializeField] private int m_maxSpawnedItems = 5; // Limits the amount of this item that the source can spawn.


    private void Update()
    {
        if (m_playerList != null)
        {
            if (m_playerList.Count > 0)
            {
                foreach (Player player in m_playerList)
                {
                    Item playerItem = player.GetItem();
                    XboxController controller = player.controller;
                    // Item playerItem = player.GetItem() Get palyer item here then use it for the checks below and increase charges ect
                    if (XCI.GetButtonDown(XboxButton.A, controller))
                    {
                        if (!m_spawnsItem)
                        {
                            foreach (E_Quality quality in playerItem.m_fixableQuality)
                            {
                                if (quality == m_willRefill)
                                {
									playerItem.ItemReset();
                                    playerItem.RefillCharges();
                                }
                            }
                        }
                        else
                        {
                            if (player.GetItem() == null)           // bullshit workaround to check if player is carring something.
                                if (m_spawnedItemList == null)
                                {
                                    Item newItemToList = Instantiate<Item>(m_itemToSpawn); // Spawns item
                                    newItemToList.Pickup(player); // Adds it to the player that pressed button
                                    m_spawnedItemList.Insert(0, newItemToList); // Adds it to it's own list so it can know how many are spawned.
                                    newItemToList.SetSpawn(this);
                                    newItemToList.m_playerList.Insert(0, player); // Adds the player to the player list so it will now check input
                                }
                                else if (m_spawnedItemList.Count >= m_maxSpawnedItems) // Needs to iterate through list
                                {
                                    if (FindUnheldObject()) // Checks if it can find a unheld object that is has spawned. if it has it will destroy it in function then create a new item and add it back to the list.
                                    {
                                        Item newItemToList = Instantiate<Item>(m_itemToSpawn); // Spawns item
                                        newItemToList.Pickup(player); // Adds it to the player that pressed button
                                        m_spawnedItemList.Insert(0, newItemToList); // Adds it to it's own list so it can know how many are spawned.
                                        newItemToList.SetSpawn(this);
                                        newItemToList.m_playerList.Insert(0, player); // Adds the player to the player list so it will now check input
                                    }
                                }
                                else
                                {
                                    Item newItemToList = Instantiate<Item>(m_itemToSpawn); // Spawns item
                                    newItemToList.Pickup(player); // Adds it to the player that pressed button
                                    m_spawnedItemList.Insert(0, newItemToList); // Adds it to it's own list so it can know how many are spawned.
                                    newItemToList.SetSpawn(this);
                                    newItemToList.m_playerList.Insert(0, player); // Adds the player to the player list so it will now check input
                                }
                            
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Switching depending on the colliders name
        int count = m_playerList.Count;

		if (other.tag == "Player")
		{
			Player holder = other.GetComponent<Player>();

			m_playerList.Insert(count, Timer.PlayerGet(holder.GetPlayerIndex()));
		}
	}

    private void OnTriggerExit(Collider other)
    {
		if (other.tag == "Player")
		{
			Player holder = other.GetComponent<Player>();

			m_playerList.Remove(Timer.PlayerGet(holder.GetPlayerIndex()));
		}
	}

    private bool FindUnheldObject()
    {
        for (int i = m_spawnedItemList.Count - 1; i > 0; i--)
        {
            if (m_spawnedItemList[i].GetHeldItem())
            {
                Item itemToRemove = m_spawnedItemList[i];
                Destroy(itemToRemove.gameObject);
                RemoveItemFromList(itemToRemove);
                return true;
            }
        }
        return false;
    }

    public void RemoveItemFromList(Item a_itemToDestroy)
    {
        // Used to remove items from list when used up when used.
        m_spawnedItemList.Remove(a_itemToDestroy);
    }


}
