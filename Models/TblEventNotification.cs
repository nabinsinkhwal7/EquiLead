using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class TblEventNotification
{
    public int ReminderId { get; set; }

    public int HourDay { get; set; }

    public string HourDayUnit { get; set; } = null!;
}
