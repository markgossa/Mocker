namespace Mocker.Domain.Models
{
    public abstract class MockAction
    {
        public string? Body { get; }
        public int Delay { get; }

        public MockAction(string? body, int delay)
        {
            Body = body;
            Delay = delay;
        }
    }
}