namespace ConsoleApp1
{
    public class Data
    {
        public Data(string sessionId, int value)
        {
            this.SessionId = sessionId;
            this.Value = value;
        }

        public string SessionId { get; }
        public int Value { get; }
    }
}
