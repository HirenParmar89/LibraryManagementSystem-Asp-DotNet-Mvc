using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.Fine;

public class FineListViewModel
{
    public IEnumerable<FineDto> Fines { get; set; } = new List<FineDto>();
}

public class FinePaymentViewModel
{
    public Guid FineId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal PaymentAmount { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
}