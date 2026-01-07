using System.ComponentModel.DataAnnotations;

namespace MabatSupportSystem.Models;

/// <summary>
/// نموذج الإشعارات - لإرسال إشعارات للمستخدمين
/// </summary>
public class Notification
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "العنوان")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    [Display(Name = "الرسالة")]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "نوع الإشعار")]
    public NotificationType Type { get; set; } = NotificationType.Info;

    [Display(Name = "تمت القراءة")]
    public bool IsRead { get; set; } = false;

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "تاريخ القراءة")]
    public DateTime? ReadDate { get; set; }

    // رابط للصفحة المتعلقة (مثل تفاصيل التذكرة)
    [StringLength(500)]
    public string? ActionUrl { get; set; }

    // المستخدم المستلم للإشعار
    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    // التذكرة المرتبطة (اختياري)
    public int? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
}

/// <summary>
/// أنواع الإشعارات
/// </summary>
public enum NotificationType
{
    [Display(Name = "معلومات")]
    Info = 0,

    [Display(Name = "تذكرة جديدة")]
    NewTicket = 1,

    [Display(Name = "رد جديد")]
    NewResponse = 2,

    [Display(Name = "تغيير الحالة")]
    StatusChanged = 3,

    [Display(Name = "تم التعيين")]
    TicketAssigned = 4,

    [Display(Name = "تقييم مطلوب")]
    RatingRequired = 5,

    [Display(Name = "تنبيه")]
    Warning = 6,

    [Display(Name = "عاجل")]
    Urgent = 7
}
