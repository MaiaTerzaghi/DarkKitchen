namespace DarkKitchen.Domain.Auditing;

public interface IAuditObserver
{
    void Update(AuditEvent auditEvent);
}
