using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// Funci�n para cargar la escena del juego, al pulsar Start o Restart
    /// </summary>
    public void StartButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// Funci�n para salir del juego, al pulsar Quit
    /// </summary>
    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }
}
