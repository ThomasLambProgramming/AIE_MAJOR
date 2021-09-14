namespace Malicious.Interactables
{
    public interface IInteractable
    {
        public void OnHackValid();
        public void OnHackFalse();
        public float HackedHoldTime();
        public bool HasHoldInput();
        public void HoldInputActivate();
        public void Hacked();
    }
}