namespace Malicious.Interfaces
{
    /// <summary>
    /// Hackable interactables are the single use hacked objects
    /// eg levers, as they only need to fire off a unity event they only need
    /// the on hacked function
    /// </summary>
    public interface IHackableInteractable 
    {
        //Just a general send that its hacked function to start the unity event
        void Hacked();
    }
}
