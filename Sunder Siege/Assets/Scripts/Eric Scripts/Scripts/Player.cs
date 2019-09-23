using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Player : MonoBehaviour
{
	public float movementSpeed;
	public float dashDistance = 100f;
	public XboxController controller;

	private Vector3 newPosition;
	private Vector3 lastPosition;
	private Rigidbody rb;
	private static bool didQueryNumOfCtrlrs = false;

    // Start is called before the first frame update
    void Start()
    {
		newPosition = transform.position;
		rb = GetComponent<Rigidbody>();

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

    // Update is called once per frame
    void Update()
    {
		movement();
    }

	void movement()
	{
		// Remember the lastPosition the Player was
		lastPosition = transform.position;

		// Left stick movement
		newPosition = transform.position;
		float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controller);
		float axisY = XCI.GetAxis(XboxAxis.LeftStickY, controller);
		float newPosX = newPosition.x + (axisX * movementSpeed * Time.deltaTime);
		float newPosZ = newPosition.z + (axisY * movementSpeed * Time.deltaTime);
		newPosition = new Vector3(newPosX, transform.position.y, newPosZ);
		transform.position = newPosition;

		// 'X' dash movement
		if (XCI.GetButtonDown(XboxButton.X, controller))
		{
			if (dashDistance == 0 || dashDistance == 1)
			{
				return;
			}
			else
			{
				Vector3 direction = transform.position - lastPosition;
				Vector3 localDirection = transform.InverseTransformDirection(direction);

				rb.AddForce(localDirection * dashDistance, ForceMode.Impulse);

				lastPosition = transform.position;

				rb.velocity = Vector3.zero;
			}
		}
	}

	void itemPickup(GameObject a_otherObject)
	{

	}

	void itemDrop()
	{

	}

	void itemThrow()
	{

	}
}
