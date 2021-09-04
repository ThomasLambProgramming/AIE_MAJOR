using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Malicious
{
    public class testingScript : MonoBehaviour
    {
        private Animator testing = null;
        // Start is called before the first frame update
        void Start()
        {
            testing = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            /*
             * I cant be bothered to put this in right now because im tired but
             *
             * if movementInput != vec zero
             *                      soh cah toa angle for camera to input 
             * target angle = mathf.atan2 (movementx, movementy) * mathf rad2deg to make degrees then + cameratransform.rot.eular.y
             * we attach the camera because of us wanting the input to be the same as the camera not specifically from the player itself
             * quaternion roation = quaterion eular 0, targetangle 0
             * then lerp the current player rotation from its current to the target angle
             */
        }
    }
}
