namespace Pulumi.Extensions.Provider.Exceptions
{
    public class ResourceTypeNotSupportedException(string resourceType) : NotSupportedException
    {
        public string ResourceType => resourceType;
    }
}
