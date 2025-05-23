namespace EquidCMS.Dto
{
    public class JobAnalyticsModel
    {
        public string? Title { get; set; }
        public int JobCount { get; set; }
        public int ClickCount { get; set; }
    }
    public class JobClickAnalyticsModel
    {
        public string CompanyName { get; set; }
        public int JobCount { get; set; }
        public int ClickCount { get; set; }
    }

}
