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

    [TestMethod]
    public void GetLogs_WhenFiltersAreValid_MapsAuditLogFieldsToResponse()
    {
        var log = new AuditLog
        {
            Id = 7,
            Timestamp = new DateTime(2026, 4, 23, 9, 0, 0),
            EntityName = AuditedEntity.Product,
            EntityId = 12345,
            Description = "Alta de producto.",
            ResponsibleUser = "admin@email.com"
        };
        _auditRepositoryMock
            .Setup(r => r.GetByEntity(It.IsAny<AuditedEntity>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(new List<AuditLog> { log });
        var request = new GetAuditLogsRequestDTO
        {
            EntityName = AuditedEntity.Product,
            EntityId = 12345,
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0)
        };

        var result = _service.GetLogs(request);

        var dto = result[0];
        Assert.AreEqual(7, dto.Id);
        Assert.AreEqual(new DateTime(2026, 4, 23, 9, 0, 0), dto.Timestamp);
        Assert.AreEqual("Product", dto.EntityName);
        Assert.AreEqual(12345, dto.EntityId);
        Assert.AreEqual("Alta de producto.", dto.Description);
        Assert.AreEqual("admin@email.com", dto.ResponsibleUser);
    }
}
