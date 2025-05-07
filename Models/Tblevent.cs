using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblevent
{
    public int Eventid { get; set; }

    public int? Themeid { get; set; }

    public int? Numberofattended { get; set; }

    public string? Descriptionofevent { get; set; }

    public DateOnly Startdateofevent { get; set; }

    public DateOnly? Enddateofevent { get; set; }

    public bool Isvalidate { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Createdon { get; set; }

    public int? Updatedby { get; set; }

    public DateTime? Updatedon { get; set; }

    public int? Evidenceid { get; set; }

    public int? EventTypeId { get; set; }

    public TimeOnly? EventTimeStart { get; set; }

    public string? EventVenue { get; set; }

    public string? EventLink { get; set; }

    public string? EventRegistrationLink { get; set; }

    public string? EventSpeaker { get; set; }

    public string? EventSpeakerOrg { get; set; }

    public string? EventHost { get; set; }

    public string? EventHostOrg { get; set; }

    public string? EventName { get; set; }

    public int? EventPricingTypeId { get; set; }

    public int? EventPricing { get; set; }

    public string? EventAgendaDoc { get; set; }

    public string? Parking { get; set; }

    public string? WheelChair { get; set; }

    public string? EventBanner { get; set; }

    public TimeOnly? EventTimeEnd { get; set; }

    public string? Eventbenefit { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual Tblevent Event { get; set; } = null!;

    public virtual MstEventpricing? EventPricingType { get; set; }

    public virtual MstEventtype? EventType { get; set; }

    public virtual Tblevent? InverseEvent { get; set; }

    public virtual ICollection<Tbleventbenefit> Tbleventbenefits { get; set; } = new List<Tbleventbenefit>();

    public virtual ICollection<Tbleventparticipant> Tbleventparticipants { get; set; } = new List<Tbleventparticipant>();

    public virtual ICollection<Tblevidence> Tblevidences { get; set; } = new List<Tblevidence>();

    public virtual MstTheme? Theme { get; set; }
}
