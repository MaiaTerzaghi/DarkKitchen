using DarkKitchen.Domain.Auditing;

namespace DarkKitchen.BusinessLogic.Auditing;

public sealed class AuditNotifier : IAuditSubject
{
    private readonly List<IAuditObserver> _observers = [];

    public void Attach(IAuditObserver observer) => _observers.Add(observer);

    public void Notify(AuditEvent auditEvent)
    {
        foreach(var observer in _observers)
        {
            observer.Update(auditEvent);
        }
    }
}
