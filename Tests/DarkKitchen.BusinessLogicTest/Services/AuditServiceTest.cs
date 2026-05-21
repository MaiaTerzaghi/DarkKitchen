using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class AuditServiceTest
{
    private AuditService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new AuditService();
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
}
