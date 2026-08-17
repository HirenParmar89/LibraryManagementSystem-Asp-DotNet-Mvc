using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public IActionResult Index()
    {
        return View();
    }

    // GET: Reports/Circulation
    public async Task<IActionResult> Circulation(DateTime? startDate, DateTime? endDate)
    {
        var result = await _reportService.GetCirculationReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        return View(result.Data ?? new List<CirculationReportDto>());
    }

    public async Task<IActionResult> ExportCirculation(DateTime? startDate, DateTime? endDate)
    {
        var result = await _reportService.GetCirculationReportAsync(startDate, endDate);
        var data = result.Data ?? new List<CirculationReportDto>();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Circulation Report");
        worksheet.Cell(1, 1).Value = "Book Title";
        worksheet.Cell(1, 2).Value = "Member";
        worksheet.Cell(1, 3).Value = "Issue Date";
        worksheet.Cell(1, 4).Value = "Due Date";
        worksheet.Cell(1, 5).Value = "Return Date";
        worksheet.Cell(1, 6).Value = "Status";
        worksheet.Cell(1, 7).Value = "Fine Amount";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < data.Count(); i++)
        {
            var item = data.ElementAt(i);
            worksheet.Cell(i + 2, 1).Value = item.BookTitle;
            worksheet.Cell(i + 2, 2).Value = item.MemberName;
            worksheet.Cell(i + 2, 3).Value = item.IssueDate.ToString("yyyy-MM-dd");
            worksheet.Cell(i + 2, 4).Value = item.DueDate.ToString("yyyy-MM-dd");
            worksheet.Cell(i + 2, 5).Value = item.ReturnDate?.ToString("yyyy-MM-dd") ?? "N/A";
            worksheet.Cell(i + 2, 6).Value = item.Status.ToString();
            worksheet.Cell(i + 2, 7).Value = item.FineAmount;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CirculationReport.xlsx");
    }

    // GET: Reports/Inventory
    public async Task<IActionResult> Inventory()
    {
        var result = await _reportService.GetInventoryReportAsync();
        return View(result.Data ?? new List<InventoryReportDto>());
    }

    public async Task<IActionResult> ExportInventory()
    {
        var result = await _reportService.GetInventoryReportAsync();
        var data = result.Data ?? new List<InventoryReportDto>();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Inventory Report");
        worksheet.Cell(1, 1).Value = "ISBN";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Author";
        worksheet.Cell(1, 4).Value = "Category";
        worksheet.Cell(1, 5).Value = "Total Copies";
        worksheet.Cell(1, 6).Value = "Available Copies";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < data.Count(); i++)
        {
            var item = data.ElementAt(i);
            worksheet.Cell(i + 2, 1).Value = item.ISBN;
            worksheet.Cell(i + 2, 2).Value = item.Title;
            worksheet.Cell(i + 2, 3).Value = item.Author;
            worksheet.Cell(i + 2, 4).Value = item.Category;
            worksheet.Cell(i + 2, 5).Value = item.TotalCopies;
            worksheet.Cell(i + 2, 6).Value = item.AvailableCopies;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InventoryReport.xlsx");
    }

    // GET: Reports/Members
    public async Task<IActionResult> Members(DateTime? startDate, DateTime? endDate)
    {
        var result = await _reportService.GetMemberReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        return View(result.Data ?? new List<MemberReportDto>());
    }

    // GET: Reports/Fines
    public async Task<IActionResult> Fines(DateTime? startDate, DateTime? endDate)
    {
        var result = await _reportService.GetFineReportAsync(startDate, endDate);
        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        return View(result.Data ?? new List<FineReportDto>());
    }
}