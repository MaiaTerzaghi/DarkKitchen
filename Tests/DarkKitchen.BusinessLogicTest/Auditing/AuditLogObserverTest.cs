using DarkKitchen.BusinessLogic.Auditing;
using DarkKitchen.Domain.Auditing;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Auditing;

[TestClass]
public sealed class AuditLogObserverTest
{
    [TestMethod]
    public void Update_WhenEventReceived_PersistsAuditLog()
    {
        var auditRepositoryMock = new Mock<IAuditRepository>();
        var observer = new AuditLogObserver(auditRepositoryMock.Object);
        var auditEvent = new AuditEvent
        {
            EntityName = AuditedEntity.Product,
            EntityId = 12,
            Description = "Alta de producto.",
            ResponsibleUser = "admin@email.com"
        };

        observer.Update(auditEvent);

        auditRepositoryMock.Verify(r => r.Add(It.Is<AuditLog>(log =>
            log.EntityName == AuditedEntity.Product &&
            log.EntityId == 12 &&
            log.Description == "Alta de producto." &&
            log.ResponsibleUser == "admin@email.com")), Times.Once);
    }

    [TestMethod]
    public void Update_WhenEventReceived_SetsTimestamp()
    {
        var auditRepositoryMock = new Mock<IAuditRepository>();
        var observer = new AuditLogObserver(auditRepositoryMock.Object);
        var before = DateTime.Now;

        observer.Update(new AuditEvent());

        auditRepositoryMock.Verify(r => r.Add(It.Is<AuditLog>(log =>
            log.Timestamp >= before)), Times.Once);
    }
}
