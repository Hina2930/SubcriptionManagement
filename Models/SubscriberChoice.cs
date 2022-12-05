namespace SubcriptionManagement.Models
{
    public class SubscriberChoice
    {
        public Guid SubscriberChoiceId { get; set; }
        public Guid SubscriberId { get; set; }
        public string Choice { get; set; }
        public int ChoiceCount { get; set; }
    }
}
