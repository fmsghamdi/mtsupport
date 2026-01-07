using System.ComponentModel.DataAnnotations;

namespace MabatSupportSystem.Models;

/// <summary>
/// نموذج الفندق - يمثل الفنادق المسجلة في النظام
/// </summary>
public class Hotel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الفندق مطلوب")]
    [StringLength(200)]
    [Display(Name = "اسم الفندق")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الفندق بالإنجليزية مطلوب")]
    [StringLength(200)]
    [Display(Name = "Hotel Name (English)")]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [StringLength(100)]
    [Display(Name = "المدينة")]
    public string? City { get; set; }

    [Phone]
    [Display(Name = "رقم الهاتف")]
    public string? Phone { get; set; }

    [EmailAddress]
    [Display(Name = "البريد الإلكتروني")]
    public string? Email { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // صاحب الفندق (الرئيسي)
    public string? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    // Navigation Properties
    public ICollection<ApplicationUser> SupportTeam { get; set; } = new List<ApplicationUser>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
