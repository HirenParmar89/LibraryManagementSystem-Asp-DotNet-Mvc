namespace LibraryManagementSystem.Application.Options;

public class LibrarySettings
{
    public string LibraryName { get; set; } = "Advanced Library Management System";
    public string LibraryAddress { get; set; } = string.Empty;
    public string LibraryPhone { get; set; } = string.Empty;
    public string LibraryEmail { get; set; } = string.Empty;
    public int DefaultLoanDurationDays { get; set; } = 14;
    public int MaxBooksPerMember { get; set; } = 5;
    public int MaxRenewals { get; set; } = 2;
    public decimal DailyFineAmount { get; set; } = 5.0m;
    public int FineGracePeriodDays { get; set; } = 0;
    public int MembershipDurationMonths { get; set; } = 12;
    public string CurrencySymbol { get; set; } = "₹";
    public bool BlockIssueOnFine { get; set; } = true;
}