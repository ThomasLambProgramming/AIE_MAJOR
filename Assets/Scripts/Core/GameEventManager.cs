using System;
using UnityEngine;

namespace Malicious.Core
{
    public class GameEventManager : MonoBehaviour
    {
        //These events is seperated to allow for more options
        //such as disabling the player update while a cutscene is active
        //without needing heaps of if statements on when the player can and 
        //cannot move
    
        /// <summary>
        /// Player update event
        /// </summary>
        public static event Action PlayerUpdate;
        //fixed update is needed as well for physics updates
        /// <summary>
        /// Player Fixed update event
        /// </summary>
        public static event Action PlayerFixedUpdate;
    
        /// <summary>
        /// Update event for all enemies
        /// </summary>
        public static event Action EnemyUpdate;
        /// <summary>
        /// Fixed Update event for all enemies
        /// </summary>
        public static event Action EnemyFixedUpdate;
    
        /// <summary>
        /// General update is for all objects needing an update loop
        /// outside of enemies and the player
        /// </summary>
        public static event Action GeneralUpdate;
        /// <summary>
        /// General update is for all objects needing an update loop
        /// outside of enemies and the player
        /// </summary>
        public static event Action GeneralFixedUpdate;

        

        void Update()
        {
            PlayerUpdate?.Invoke();
            EnemyUpdate?.Invoke();
            GeneralUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            PlayerFixedUpdate?.Invoke();
            EnemyFixedUpdate?.Invoke();
            GeneralFixedUpdate?.Invoke();
        }

        /// <summary>
        /// THIS IS ONLY TO BE USED WHEN ABSOLUTELY NEEDED
        /// MAINLY FOR DEBUG
        /// </summary>
        public void ClearAllEvents()
        {
            PlayerUpdate        = null;
            PlayerFixedUpdate   = null;
            EnemyUpdate         = null;
            EnemyFixedUpdate    = null;
            GeneralUpdate       = null;
            GeneralFixedUpdate  = null;
        }
    }
}
