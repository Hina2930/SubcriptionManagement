namespace SubcriptionManagement.Models
{
    public class Response
    {
        public int TotalSubscriber { get; set; }
        public int FemaleSubscriber { get; set; }

        public int MaleSubscriber { get; set; }
        public int CurrentYearSubscriber { get; set; }

        public List<SelectiveSubscriber> Subscribers { get; set; }

        public Report Report { get; set; }
    }

    public class Report
    {
        public int TotalMale { get; set;}
        public int TotalFemale { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentYearSubscriber { get; set; }

    }
    public class GenderSpecificDetail
        {
        public bool Gender { get; set; }
        public int TotalPrice { get; set; }

    }

    public class SelectiveSubscriber
    {
        public string Name { get; set; }
        public bool Gender { get; set; }
        public int Age { get; set; }
    }
}
