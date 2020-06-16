namespace CDEM.Shared
{
    public class Subscription
    {
        public Subscription(string[] events)
        {
            Events = events;
        }
        public string[] Events { get; }
    }
}
