using EquidCMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EquidCMS.Common
{
    public class InfographicViewModel
    {
        public List<Tblinfographic> Records { get; set; } // Existing records
        public Tblinfographic NewEntry { get; set; } // New entry model for form
    }
}
