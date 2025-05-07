using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblresource
{
    public int Resourceid { get; set; }

    public string? Rshead { get; set; }

    public string? Rsimage { get; set; }

    public string? Rsshortdescription { get; set; }

    public int? Rsage { get; set; }

    public int? ThemeId { get; set; }

    public bool Isverified { get; set; }

    public bool? Isrelatedrs { get; set; }

    public bool Isdeleted { get; set; }

    public bool Ispublic { get; set; }

    public string? Rsdocument { get; set; }

    public bool? Rsversion { get; set; }

    public int? Rsdocumenttypeid { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Createdon { get; set; }

    public int? Updatedby { get; set; }

    public DateTime? Updatedon { get; set; }

    public List<int>? Topic { get; set; }

    public string? RsvideoLink { get; set; }

    public int? CategoryId { get; set; }

    public virtual MstRsdocumenttype? Rsdocumenttype { get; set; }

    public virtual MstTheme? Theme { get; set; }
}
