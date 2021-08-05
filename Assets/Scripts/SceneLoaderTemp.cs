using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoaderTemp : MonoBehaviour
{
    private MasterInput inputCheat;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            
        inputCheat = new MasterInput();
        inputCheat.Enable();
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            inputCheat.Player.Click.performed += CheatClick;
        }
    }

    private void CheatClick(InputAction.CallbackContext a_context)
    {
        LoadFirst();
    }

    public void LoadFirst()
    {
        SceneManager.LoadScene(1);
        
    }
    public void LoadSecond()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
