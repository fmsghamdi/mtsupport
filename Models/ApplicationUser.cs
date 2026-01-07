using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MabatSupportSystem.Models;

/// <summary>
/// Application user extending Identity user
/// يمثل المستخدم في النظام بكل أنواعه
/// </summary>
public class ApplicationUser : IdentityUser
{
    [StringLength(200)]
    [Display(Name = "الاسم الكامل")]
    public string? FullName { get; set; }

    [Display(Name = "نوع المستخدم")]
    public UserType UserType { get; set; } = UserType.EndUser;

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "آخر تسجيل دخول")]
    public DateTime? LastLoginDate { get; set; }

    [StringLength(500)]
    [Display(Name = "صورة الملف الشخصي")]
    public string? ProfileImageUrl { get; set; }

    // الفندق الذي ينتمي إليه (لفريق دعم الفندق أو صاحب الفندق)
    public int? HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    // Navigation Properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    public ICollection<TicketResponse> Responses { get; set; } = new List<TicketResponse>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<TicketRating> GivenRatings { get; set; } = new List<TicketRating>();
    public ICollection<TicketRating> ReceivedRatings { get; set; } = new List<TicketRating>();
}

/// <summary>
/// أنواع المستخدمين في النظام
/// </summary>
public enum UserType
{
    [Display(Name = "مستخدم نهائي")]
    EndUser = 0,

    [Display(Name = "صاحب فندق")]
    HotelOwner = 1,

    [Display(Name = "فريق دعم الفندق")]
    HotelSupport = 2,

    [Display(Name = "فريق دعم مبات")]
    MabatSupport = 3,

    [Display(Name = "مدير النظام")]
    Admin = 4
}
