using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious.Interfaces
{
    public interface IHackable
    {
        public void Hacked();
        //This can be null for interactable
        public void PlayerExit();
    }
}


/*
 * Movement scheme changing from player to moveable or wire
 * function for each
 * or just running unity event
 * that way i can just have if interactable != null interactable.hacked();
 *
 *
 *
 * ----PLAYER-----
 * Enter wire, Exit Wire, Wire Jump Override
 *
 * Movement function for ground, flying and block with options for designers
 */
