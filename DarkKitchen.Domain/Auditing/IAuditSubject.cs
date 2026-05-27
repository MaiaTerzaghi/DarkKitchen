namespace DarkKitchen.Domain.Auditing;

public interface IAuditSubject
{
    void Notify(AuditEvent auditEvent);
}
