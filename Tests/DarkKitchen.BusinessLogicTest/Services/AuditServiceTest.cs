using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class AuditServiceTest
{
    private Mock<IAuditRepository> _auditRepositoryMock = null!;
    private AuditService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _auditRepositoryMock = new Mock<IAuditRepository>();
        _service = new AuditService(_auditRepositoryMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenDateFromIsMissing_ThrowsArgumentException()
    {
        var request = new GetAuditLogsRequestDTO();

        _service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenDateToIsMissing_ThrowsArgumentException()
    {
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0)
        };

        _service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenDateFromIsNotLessThanDateTo_ThrowsArgumentException()
    {
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 10, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 8, 0, 0)
        };

        _service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenEntityNameIsMissing_ThrowsArgumentException()
    {
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0)
        };

        _service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenEntityIdIsMissing_ThrowsArgumentException()
    {
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0),
            EntityName = AuditedEntity.Product
        };

        _service.GetLogs(request);
    }

    [TestMethod]
    public void GetLogs_WhenFiltersAreValid_ReturnsLogsFromRepository()
    {
        var logs = new List<AuditLog>
        {
            new AuditLog { Id = 1 }
        };
        _auditRepositoryMock
            .Setup(r => r.GetByEntity(AuditedEntity.Product, 12345, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(logs);
        var request = new GetAuditLogsRequestDTO
        {
            EntityName = AuditedEntity.Product,
            EntityId = 12345,
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0)
        };

        var result = _service.GetLogs(request);

        Assert.AreEqual(1, result.Count);
    }
}
