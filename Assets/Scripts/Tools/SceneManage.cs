using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Malicious.Tools
{
    public class SceneManage : MonoBehaviour
    {
        public void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void LoadLevelOne(int a_index)
        {
            SceneManager.LoadScene(a_index);
        }
    }
}
