namespace EquidCMS.Dto
{
    public class SPMailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class EmailRequestModel
    {
        public List<SPMailModel> Recipients { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string CC { get; set; }
    }
}

