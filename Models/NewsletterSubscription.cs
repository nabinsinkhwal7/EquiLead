using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class NewsletterSubscription
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public bool? Subscribed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
