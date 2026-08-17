namespace LibraryManagementSystem.Application.ViewModels;

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int IssuedCopies { get; set; }
    public int OverdueLoans { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int PendingReservations { get; set; }
    public decimal OutstandingFines { get; set; }
    public int TodaysIssues { get; set; }
    public int TodaysReturns { get; set; }
}

public class ChartDataViewModel
{
    public List<string> Labels { get; set; } = new();
    public List<int> Data { get; set; } = new();
}