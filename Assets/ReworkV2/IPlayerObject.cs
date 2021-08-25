using System;
using UnityEngine;

namespace Malicious.ReworkV2
{
    /// <summary>
    /// All classes contracted are "Playable" for the player to control in some form
    /// </summary>
    public interface IPlayerObject
    {
        public void OnHackEnter();
        public void OnHackExit();

        //Update Loop (this is seperated so it can be stopped and started when needed)
        public void Tick();
        public void FixedTick();

        //Camera Offset functions
        public OffsetContainer GiveOffset();
        public bool RequiresOffset();
        public void SetOffset(Transform a_offset);
        
        //for all materials and other graphical changes when the player can hack
        public void OnHackValid();
        public void OnHackFalse();

        //Controlled Game Object functions
        //public GameObject GiveObject();
        //public void SetObject(GameObject a_gameObject);
    }
    [Serializable]
    public class OffsetContainer
    {
        [SerializeField] public Transform _offsetTransform = null;
        [SerializeField] public Vector3 _rigOffset = Vector3.zero;
    }
}