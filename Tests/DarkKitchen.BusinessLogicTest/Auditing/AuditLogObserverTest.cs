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
    private const string AltaDeProducto = "Alta de producto.";
    private const string AdminEmailCom = "admin@email.com";

    [TestMethod]
    public void Update_WhenEventReceived_PersistsAuditLog()
    {
        var auditRepositoryMock = new Mock<IAuditRepository>();
        var observer = new AuditLogObserver(auditRepositoryMock.Object);
        var auditEvent = new AuditEvent
        {
            EntityName = AuditedEntity.Product,
            EntityId = 12,
            Description = AltaDeProducto,
            ResponsibleUser = AdminEmailCom
        };

        observer.Update(auditEvent);

        auditRepositoryMock.Verify(r => r.Add(It.Is<AuditLog>(log =>
            log.EntityName == AuditedEntity.Product &&
            log.EntityId == 12 &&
            log.Description == AltaDeProducto &&
            log.ResponsibleUser == AdminEmailCom)), Times.Once);
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
