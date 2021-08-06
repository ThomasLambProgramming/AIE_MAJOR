using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SceneLoaderTemp : MonoBehaviour
{
    
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
