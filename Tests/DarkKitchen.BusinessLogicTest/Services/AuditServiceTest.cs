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
}
