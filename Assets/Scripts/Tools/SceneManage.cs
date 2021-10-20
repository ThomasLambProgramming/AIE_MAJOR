using System.Collections;
using System.Collections.Generic;
using Malicious.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Malicious.Tools
{
    public class SceneManage : MonoBehaviour
    {
        public void LoadMenu()
        {
            GameEventManager.Reset();
            SceneManager.LoadScene(0);
        }

        public void LoadLevelOne(int a_index)
        {
            SceneManager.LoadScene(a_index);
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        public void LoadCurrentLevel()
        {
            GameEventManager.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
