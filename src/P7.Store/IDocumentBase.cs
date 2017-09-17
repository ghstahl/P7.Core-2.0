using System;

namespace P7.Store
{
    public interface IDocumentBase
    {
        Guid Id_G { get; }
        string Id { get; }
    }
    public interface IDocumentBaseWithTenant: IDocumentBase
    {
        Guid TenantId_G { get; }
        string TenantId { get; }
    }
}