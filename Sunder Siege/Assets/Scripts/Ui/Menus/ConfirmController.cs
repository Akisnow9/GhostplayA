using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class ConfirmController : MonoBehaviour
{
	[Header("Store the states of confirmation, needs to confirm, ready, etc.")]
	public List<GameObject> m_confirmationState;

	static private ConfirmController instance = null;

	private void Start()
	{
		instance = this;
	}

	public void ChangeConfirmState(bool a_isReady)
	{
		if (a_isReady)
		{
			m_confirmationState[0].SetActive(false);
			m_confirmationState[1].SetActive(true);
		}
	}
}
