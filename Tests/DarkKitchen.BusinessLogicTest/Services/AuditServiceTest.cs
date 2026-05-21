using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DTOs.Args.In;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class AuditServiceTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenDateFromIsMissing_ThrowsArgumentException()
    {
        var service = new AuditService();
        var request = new GetAuditLogsRequestDTO();

        service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenDateToIsMissing_ThrowsArgumentException()
    {
        var service = new AuditService();
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0)
        };

        service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenDateFromIsNotLessThanDateTo_ThrowsArgumentException()
    {
        var service = new AuditService();
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 10, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 8, 0, 0)
        };

        service.GetLogs(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetLogs_WhenEntityNameIsMissing_ThrowsArgumentException()
    {
        var service = new AuditService();
        var request = new GetAuditLogsRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0)
        };

        service.GetLogs(request);
    }
}
