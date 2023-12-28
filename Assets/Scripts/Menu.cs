using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
