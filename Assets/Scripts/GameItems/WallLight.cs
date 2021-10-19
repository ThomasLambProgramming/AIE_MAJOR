using UnityEngine;

namespace Malicious.GameItems
{
    public class WallLight : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer = null;
        [SerializeField] private Material _nonHackedMaterial = null;
        [SerializeField] private Material _hackedMaterial = null;

        [ContextMenu("HackTest")]
        public void ChangeToRed()
        {
            _renderer.material = _hackedMaterial;
        }

        public void ChangeToGreen()
        {
            _renderer.material = _nonHackedMaterial;
        }
    }
}