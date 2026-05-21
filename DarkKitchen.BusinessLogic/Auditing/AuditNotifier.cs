using DarkKitchen.Domain.Auditing;

namespace DarkKitchen.BusinessLogic.Auditing;

public sealed class AuditNotifier : IAuditSubject
{
    private IAuditObserver? _observer;

    public void Attach(IAuditObserver observer) => _observer = observer;

    public void Notify(AuditEvent auditEvent) => _observer?.Update(auditEvent);
}
