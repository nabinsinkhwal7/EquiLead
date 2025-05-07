namespace EquidCMS.Dto
{
    public class LookupDTO
    {
        public int LookupFlag { get; set; }
        public int LookupCode { get; set; }
        public string Description { get; set; }
        public string HintDetails { get; set; }
        public int SeqNo { get; set; }
        public bool Active { get; set; }
    }
    public class LookupStatusUpdateDTO
    {
        public int lookupCode { get; set; }
        public string selectedHintDetails { get; set; }
        public bool isActive { get; set; }
    }
    public class LookupUpdateDTO
    {
        public int lookupCode { get; set; }
        public string selectedHintDetails { get; set; }
        public string description { get; set; }
    }
    public class LookupCreateDto
    {
        public string selectedHintDetails { get; set; }
        public string Description { get; set; }
    }

}
