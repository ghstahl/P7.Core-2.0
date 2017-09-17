namespace P7.IdentityServer4.Common
{
    public interface ISimpleJsonDocument
    {
        object Document { get; }
        string DocumentJson { get; }
    }
}