using DarkKitchen.BusinessLogic.Auditing;
using DarkKitchen.Domain.Auditing;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Auditing;

[TestClass]
public sealed class AuditNotifierTest
{
    [TestMethod]
    public void Notify_WhenObserverAttached_CallsUpdateOnObserver()
    {
        var observerMock = new Mock<IAuditObserver>();
        var notifier = new AuditNotifier();
        notifier.Attach(observerMock.Object);
        var auditEvent = new AuditEvent();

        notifier.Notify(auditEvent);

        observerMock.Verify(o => o.Update(auditEvent), Times.Once);
    }
}
