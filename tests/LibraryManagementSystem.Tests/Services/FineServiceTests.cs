namespace LibraryManagementSystem.Tests.Services;

public class FineServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepositoryMock;
    private readonly Mock<IFineRepository> _fineRepositoryMock;
    private readonly Mock<IFinePaymentRepository> _finePaymentRepositoryMock;
    private readonly Mock<IMemberRepository> _memberRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly FineService _fineService;

    public FineServiceTests()
    {
        _loanRepositoryMock = new Mock<ILoanRepository>();
        _fineRepositoryMock = new Mock<IFineRepository>();
        _finePaymentRepositoryMock = new Mock<IFinePaymentRepository>();
        _memberRepositoryMock = new Mock<IMemberRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _fineService = new FineService(
            _loanRepositoryMock.Object,
            _fineRepositoryMock.Object,
            _finePaymentRepositoryMock.Object,
            _memberRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task WaiveFineAsync_ShouldFail_WhenFineIsAlreadyPaid()
    {
        // Arrange
        var fineId = Guid.NewGuid();
        var fine = new Fine 
        { 
            Id = fineId, 
            Amount = 50, 
            PaymentStatus = FinePaymentStatus.Paid 
        };

        _fineRepositoryMock.Setup(r => r.GetByIdAsync(fineId)).ReturnsAsync(fine);

        // Act
        var result = await _fineService.WaiveFineAsync(fineId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Cannot waive a fully paid fine");
    }

    [Fact]
    public async Task RecordPaymentAsync_ShouldMarkAsPaid_WhenFullAmountIsPaid()
    {
        // Arrange
        var fineId = Guid.NewGuid();
        var fine = new Fine 
        { 
            Id = fineId, 
            Amount = 50, 
            PaidAmount = 0, 
            RemainingAmount = 50, 
            PaymentStatus = FinePaymentStatus.Pending 
        };

        _fineRepositoryMock.Setup(r => r.GetByIdAsync(fineId)).ReturnsAsync(fine);

        var paymentDto = new FinePaymentDto(fineId, 50m, PaymentMethod.Cash, "teller");

        // Act
        var result = await _fineService.RecordPaymentAsync(paymentDto);

        // Assert
        result.Success.Should().BeTrue();
        fine.PaidAmount.Should().Be(50);
        fine.RemainingAmount.Should().Be(0);
        fine.PaymentStatus.Should().Be(FinePaymentStatus.Paid);
        fine.PaidDate.Should().NotBeNull();
    }
}