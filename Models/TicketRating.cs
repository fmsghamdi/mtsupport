using System.ComponentModel.DataAnnotations;

namespace MabatSupportSystem.Models;

/// <summary>
/// نموذج تقييم التذاكر - يتيح للمستخدم تقييم جودة الدعم الفني
/// </summary>
public class TicketRating
{
    public int Id { get; set; }

    /// <summary>
    /// التقييم من 1 إلى 5 نجوم
    /// </summary>
    [Required(ErrorMessage = "التقييم مطلوب")]
    [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
    [Display(Name = "التقييم")]
    public int Rating { get; set; }

    /// <summary>
    /// تعليق المستخدم على الخدمة
    /// </summary>
    [StringLength(1000)]
    [Display(Name = "تعليقك على الخدمة")]
    public string? Comment { get; set; }

    /// <summary>
    /// هل كان الحل مفيداً؟
    /// </summary>
    [Display(Name = "هل كان الحل مفيداً؟")]
    public bool? WasSolutionHelpful { get; set; }

    /// <summary>
    /// سرعة الاستجابة (1-5)
    /// </summary>
    [Range(1, 5)]
    [Display(Name = "سرعة الاستجابة")]
    public int? ResponseSpeedRating { get; set; }

    /// <summary>
    /// احترافية الموظف (1-5)
    /// </summary>
    [Range(1, 5)]
    [Display(Name = "احترافية الموظف")]
    public int? ProfessionalismRating { get; set; }

    [Display(Name = "تاريخ التقييم")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // التذكرة المُقيّمة
    [Required]
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    // المستخدم الذي قدم التقييم
    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    // موظف الدعم الذي تم تقييمه (اختياري)
    public string? SupportAgentId { get; set; }
    public ApplicationUser? SupportAgent { get; set; }
}
