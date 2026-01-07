using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MabatSupportSystem.Models;

/// <summary>
/// Represents a support ticket for the Mabat hotel booking system
/// يمثل تذكرة دعم فني في نظام مبات
/// </summary>
public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان التذكرة مطلوب")]
    [MaxLength(200)]
    [Display(Name = "العنوان")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "وصف المشكلة مطلوب")]
    [MaxLength(2000)]
    [Display(Name = "الوصف")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "الحالة")]
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    [Required]
    [Display(Name = "الأولوية")]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required]
    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "تاريخ التحديث")]
    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "تاريخ الإغلاق")]
    public DateTime? ClosedDate { get; set; }

    /// <summary>
    /// Reference to the Mabat booking system - nullable for general inquiries
    /// </summary>
    [MaxLength(100)]
    [Display(Name = "رقم مرجع الحجز")]
    public string? BookingReferenceId { get; set; }

    // ========== User Relations ==========
    
    /// <summary>
    /// المستخدم الذي أنشأ التذكرة
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;

    [MaxLength(100)]
    [Display(Name = "اسم المستخدم")]
    public string? UserName { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? ApplicationUser { get; set; }

    /// <summary>
    /// موظف الدعم المعيّن للتذكرة
    /// </summary>
    public string? AssignedToId { get; set; }

    [ForeignKey(nameof(AssignedToId))]
    public ApplicationUser? AssignedTo { get; set; }

    [Display(Name = "تاريخ التعيين")]
    public DateTime? AssignedDate { get; set; }

    // ========== Category Relation ==========
    
    [Required]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public TicketCategory? Category { get; set; }

    // ========== Hotel Relation ==========
    
    /// <summary>
    /// الفندق المرتبط بالتذكرة (اختياري)
    /// </summary>
    public int? HotelId { get; set; }

    [ForeignKey(nameof(HotelId))]
    public Hotel? Hotel { get; set; }

    // ========== Internal Notes ==========
    
    /// <summary>
    /// ملاحظات داخلية للموظفين فقط
    /// </summary>
    [MaxLength(2000)]
    [Display(Name = "ملاحظات داخلية")]
    public string? InternalNotes { get; set; }

    // ========== Navigation Properties ==========
    
    public ICollection<TicketResponse> Responses { get; set; } = new List<TicketResponse>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    /// <summary>
    /// تقييم التذكرة (واحد لكل تذكرة)
    /// </summary>
    public TicketRating? Rating { get; set; }
}
