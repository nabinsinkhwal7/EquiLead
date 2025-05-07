namespace EquidCMS.Models.KeyLessModels
{
    public class ResourceExport
    {
        public int SN { get; set; }
        public string Title { get; set; }
        public string Topics { get; set; }
        public string Category { get; set; }
        public string DocumentType { get; set; }
        public string Description { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsRelatedRs { get; set; }
        public bool IsPublic { get; set; }
        public string? RSVideoLink { get; set; }
    }

}
