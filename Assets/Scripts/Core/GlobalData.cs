using System;
using UnityEngine;
using System.IO;

namespace Malicious.Core
{
    /// <summary>
    /// This class is to avoid recreating MasterInput variables and etc
    /// </summary>
    [Serializable]
    public class CameraSettings
    {
        public float CameraXSpeed = 150f;
        public float CameraYSpeed = 25f;
        public bool InvertX = false;
        public bool InvertY = true;
    }
    public class GlobalData : MonoBehaviour
    {
        public static MasterInput InputManager;

        public static CameraSettings _cameraSettings;

        public static CameraSettings RetrieveSettings()
        {
            string dataPath = Application.dataPath + "/CameraSettings.json";
            if (!File.Exists(dataPath))
            {
                CameraSettings newSettings = new CameraSettings();
                StreamWriter stream = new StreamWriter(dataPath);
                string json = JsonUtility.ToJson(newSettings, true);
                stream.Write(json);
                stream.Close();
                return newSettings;
            }
            else
            {
                StreamReader stream = new StreamReader(dataPath);
                string json = stream.ReadToEnd();
                CameraSettings newSettings = JsonUtility.FromJson<CameraSettings>(json);
                stream.Close();
                return newSettings;
            }
        }
        public static void SaveSettings()
        {
            string dataPath = Application.dataPath + "/CameraSettings.json";
            if (_cameraSettings == null)
                return;
            
            StreamWriter stream = new StreamWriter(dataPath);
            string json = JsonUtility.ToJson(_cameraSettings, true);
            stream.Write(json);
            stream.Close();
        }

        private void Awake()
        {
            _cameraSettings = RetrieveSettings();
        }

        public static void MakeNewInput()
        {
            InputManager = new MasterInput();
        }

        public static void EnableInputMaster()
        {
            InputManager.Enable();
            InputManager.Player.Enable();
        }
    }
}