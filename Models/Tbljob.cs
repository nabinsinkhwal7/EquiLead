using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tbljob
{
    public int Jobid { get; set; }

    public string Jobtitle { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Workmode { get; set; }

    public int? Yearexperience { get; set; }

    public DateOnly? Dateposted { get; set; }

    public string? Roleoverview { get; set; }

    public string? Keyresponsibility { get; set; }

    public string? Skill { get; set; }

    public string? Education { get; set; }

    public string? Experience { get; set; }

    public string? Jobpostinglink { get; set; }

    public DateOnly? Applicationdeadline { get; set; }

    public int? Companyid { get; set; }

    public int? Employeetype { get; set; }

    public string? City { get; set; }

    public int? Minsalary { get; set; }

    public int? Maxsalary { get; set; }

    public List<int>? Functionalcategory { get; set; }

    public int? Flexibleworkoption { get; set; }

    public List<int>? Leavepolicies { get; set; }

    public List<int>? Learninganddeveloment { get; set; }

    public List<int>? Healthcareandwellness { get; set; }

    public List<int>? Deiandwomenfriendlypolicies { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual Tblcompany? Company { get; set; }
}
