using LibraryManagementSystem.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Web.ViewModels.Loan;

public class IssueBookViewModel
{
    [Required(ErrorMessage = "Please select a member")]
    public Guid MemberId { get; set; }

    [Required(ErrorMessage = "Please select a book copy")]
    public Guid BookCopyId { get; set; }
}

public class LoanListViewModel
{
    public IEnumerable<LoanDto> Loans { get; set; } = new List<LoanDto>();
    public string? SearchTerm { get; set; }
}