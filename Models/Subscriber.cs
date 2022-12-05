using System.ComponentModel.DataAnnotations;

namespace SubcriptionManagement.Models
{
    public class Subscriber
    {
        [Key]
        public Guid SubscriberId { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
        public int Age { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public int Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SubscriberChoice> SubscriberChoices { get; set; } = new List<SubscriberChoice>();
    }

}
