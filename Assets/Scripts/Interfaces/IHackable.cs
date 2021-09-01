namespace Malicious.Interfaces
{
    
    public interface IHackable
    {
        public void Hacked();
        
        //for all materials and other graphical changes when the player can hack
        public void OnHackValid();
        public void OnHackFalse();
    }
}
