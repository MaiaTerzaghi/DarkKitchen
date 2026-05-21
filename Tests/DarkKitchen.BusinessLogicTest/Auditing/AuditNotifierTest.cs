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

    [TestMethod]
    public void Notify_WhenMultipleObservers_CallsUpdateOnAll()
    {
        var firstObserver = new Mock<IAuditObserver>();
        var secondObserver = new Mock<IAuditObserver>();
        var notifier = new AuditNotifier();
        notifier.Attach(firstObserver.Object);
        notifier.Attach(secondObserver.Object);

        notifier.Notify(new AuditEvent());

        firstObserver.Verify(o => o.Update(It.IsAny<AuditEvent>()), Times.Once);
        secondObserver.Verify(o => o.Update(It.IsAny<AuditEvent>()), Times.Once);
    }

    [TestMethod]
    public void Notify_WhenObserversInjectedInConstructor_CallsUpdate()
    {
        var observerMock = new Mock<IAuditObserver>();
        var notifier = new AuditNotifier([observerMock.Object]);

        notifier.Notify(new AuditEvent());

        observerMock.Verify(o => o.Update(It.IsAny<AuditEvent>()), Times.Once);
    }
}
