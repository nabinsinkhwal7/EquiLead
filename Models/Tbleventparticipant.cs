using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tbleventparticipant
{
    public int Participantid { get; set; }

    public int? Eventid { get; set; }

    public string? Name { get; set; }

    public string? Emailid { get; set; }

    public string? Mobileno { get; set; }

    public string? Linkedin { get; set; }

    public virtual Tblevent? Event { get; set; }
}
