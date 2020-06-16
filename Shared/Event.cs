namespace CDEM.Shared
{
    public class Event
    {
        public Event(string name, dynamic[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public dynamic[] Arguments { get; }
    }
}
