namespace AmarenderReddy
{
    public class DummyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty!;

        public string Cron { get; set; } = string.Empty!;

        public bool Publish { get; set; }

        public DateTimeOffset? LastTriggerTime { get; set; }
    }
}