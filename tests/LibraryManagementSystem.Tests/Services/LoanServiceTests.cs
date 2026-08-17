namespace LibraryManagementSystem.Tests.Services;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepositoryMock;
    private readonly Mock<IBookCopyRepository> _bookCopyRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IMemberRepository> _memberRepositoryMock;
    private readonly Mock<IFineRepository> _fineRepositoryMock;
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IFineService> _fineServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IOptions<LibrarySettings> _librarySettings;
    private readonly LoanService _loanService;

    public LoanServiceTests()
    {
        _loanRepositoryMock = new Mock<ILoanRepository>();
        _bookCopyRepositoryMock = new Mock<IBookCopyRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _memberRepositoryMock = new Mock<IMemberRepository>();
        _fineRepositoryMock = new Mock<IFineRepository>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _fineServiceMock = new Mock<IFineService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var settings = new LibrarySettings
        {
            DefaultLoanDurationDays = 14,
            MaxBooksPerMember = 5,
            DailyFineAmount = 10.0m,
            FineGracePeriodDays = 0,
            BlockIssueOnFine = true
        };
        _librarySettings = Options.Create(settings);

        _loanService = new LoanService(
            _loanRepositoryMock.Object,
            _bookCopyRepositoryMock.Object,
            _bookRepositoryMock.Object,
            _memberRepositoryMock.Object,
            _fineRepositoryMock.Object,
            _reservationRepositoryMock.Object,
            _fineServiceMock.Object,
            _unitOfWorkMock.Object,
            _librarySettings);
    }

    [Fact]
    public async Task IssueBookAsync_ShouldFail_WhenMemberHasReachedMaxBooksLimit()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member 
        { 
            Id = memberId, 
            IsActive = true, 
            MembershipExpiryDate = DateTime.UtcNow.AddDays(10),
            MaxBooksAllowed = 5 
        };

        _memberRepositoryMock.Setup(r => r.GetByIdAsync(memberId)).ReturnsAsync(member);
        _loanRepositoryMock.Setup(r => r.GetActiveLoanCountByMemberAsync(memberId)).ReturnsAsync(5);

        var dto = new IssueBookDto(Guid.NewGuid(), memberId, "librarian");

        // Act
        var result = await _loanService.IssueBookAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("maximum borrowing limit");
    }

    [Fact]
    public async Task IssueBookAsync_ShouldFail_WhenMembershipHasExpired()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member 
        { 
            Id = memberId, 
            IsActive = true, 
            MembershipExpiryDate = DateTime.UtcNow.AddDays(-1), // Expired yesterday
            MaxBooksAllowed = 5 
        };

        _memberRepositoryMock.Setup(r => r.GetByIdAsync(memberId)).ReturnsAsync(member);
        _loanRepositoryMock.Setup(r => r.GetActiveLoanCountByMemberAsync(memberId)).ReturnsAsync(0);

        var dto = new IssueBookDto(Guid.NewGuid(), memberId, "librarian");

        // Act
        var result = await _loanService.IssueBookAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Membership has expired");
    }

    [Fact]
    public async Task ReturnBookAsync_ShouldCalculateFine_WhenBookIsOverdue()
    {
        // Arrange
        var loanId = Guid.NewGuid();
        var bookCopyId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var loan = new Loan
        {
            Id = loanId,
            BookCopyId = bookCopyId,
            MemberId = Guid.NewGuid(),
            IssueDate = DateTime.UtcNow.AddDays(-30),
            DueDate = DateTime.UtcNow.AddDays(-10), // 10 days overdue
            ReturnDate = null,
            Status = LoanStatus.Issued,
            BookCopy = new BookCopy 
            { 
                Id = bookCopyId, 
                BookId = bookId, 
                Status = BookCopyStatus.Issued, 
                IsAvailable = false 
            },
            Member = new Member()
        };

        _loanRepositoryMock.Setup(r => r.GetLoanWithDetailsAsync(loanId)).ReturnsAsync(loan);
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(new Book { Id = bookId, AvailableCopies = 0 });
        _fineServiceMock.Setup(s => s.GenerateFineForOverdueLoanAsync(loanId)).ReturnsAsync(ServiceResult.Succeeded());

        var dto = new ReturnBookDto(loanId, "librarian");

        // Act
        var result = await _loanService.ReturnBookAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        // 10 days overdue * 10.0 daily fine = 100.0
        result.Data!.FineAmount.Should().Be(100.0m);
        
        // Verify book copy was marked available again
        loan.BookCopy!.Status.Should().Be(BookCopyStatus.Available);
        loan.BookCopy.IsAvailable.Should().BeTrue();
        loan.Status.Should().Be(LoanStatus.Returned);
    }
}