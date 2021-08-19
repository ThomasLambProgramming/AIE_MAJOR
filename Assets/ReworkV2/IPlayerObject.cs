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
        public Transform GiveOffset();
        public void SetOffset(Transform a_offset);

        //Controlled Game Object functions
        //public GameObject GiveObject();
        //public void SetObject(GameObject a_gameObject);
    }
}