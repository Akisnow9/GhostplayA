using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class PauseMenu : MonoBehaviour
{
	private bool m_gameIsPaused;
	[SerializeField] private GameObject m_pauseMenuUI;
	[SerializeField] private List<GameObject> m_playerPaused;

    // Update is called once per frame
    void Update()
    {
		for (int i = 0; i < Menus.GetActivatedPlayerAmount(); i++)
		{
			StaticVariables player = Menus.GetPlayerInformation(i);

			if (XCI.GetButtonDown(XboxButton.Start, player.Controller))
			{
				if (m_gameIsPaused)
				{
					Resume();
				}
				else
				{
					Pause(player.Player);
				}
			}
		}
    }

	public void Resume()
	{
		m_pauseMenuUI.SetActive(false);

		Time.timeScale = 1f;

		for (int i = 0; i < m_playerPaused.Count; i++)
		{
			m_playerPaused[i].SetActive(false);
		}


		m_gameIsPaused = false;
	}

	private void Pause(int a_player)
	{
		m_pauseMenuUI.SetActive(true);
		m_playerPaused[a_player].SetActive(true);

		Time.timeScale = 0f;

		m_gameIsPaused = true;
	}

	public void ReturnToMenu()
	{
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}
}
