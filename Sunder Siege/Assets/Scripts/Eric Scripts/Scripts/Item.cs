using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Item : MonoBehaviour
{
	public bool canHold = true;
	public float throwForce = 600;
	public GameObject item;
	public GameObject tempParent;

	private Vector3 itemPositon;
	private float distance;
	private bool isHolding = false;

    // Update is called once per frame
    void Update()
    {
		if (isHolding)
		{
			item.GetComponent<Rigidbody>().velocity = Vector3.zero;
			item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			item.transform.SetParent(tempParent.transform);

			if (XCI.GetButtonDown(XboxButton.Y))
			{
				// Throw
			}
			else
			{
				itemPositon = item.transform.position;
				item.transform.SetParent(null);
				item.GetComponent<Rigidbody>().useGravity = true;
				item.transform.position = itemPositon;
			}
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		isHolding = true;
		item.GetComponent<Rigidbody>().useGravity = false;
		item.GetComponent<Rigidbody>().detectCollisions = true;
	}

	private void OnTriggerExit(Collider other)
	{
		isHolding = false;
	}
}
