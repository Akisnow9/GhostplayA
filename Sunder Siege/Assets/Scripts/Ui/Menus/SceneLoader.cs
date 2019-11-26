using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class SceneLoader : MonoBehaviour
{
	private bool m_loadScene = false;
	[SerializeField] private float m_timeToWait;
	private float m_waiting;
	public GameObject m_imageToDisplay;
	
    // Start is called before the first frame update
    void Awake()
    {
		m_waiting = Time.time + m_timeToWait;
    }

    // Update is called once per frame
    void Update()
    {
		//Debug.Log("Waiting " + m_waiting);
		//Debug.Log("Time " + Time.time);
		if (m_waiting < Time.time)
		{
			m_imageToDisplay.SetActive(true);

			for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
			{
				StaticVariables characterBeingChecked = Menus.GetPlayerInformation(i);
				if (XCI.GetButtonDown(XboxButton.Start, characterBeingChecked.Controller))
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
				}
			}
		}
	}
}
