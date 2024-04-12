namespace Pulumi.Extensions.Provider.Exceptions
{
    public class NotSupportedPropertyConversionException : NotSupportedException
    {
        public NotSupportedPropertyConversionException(Type sourceType, Type propertyType)
            : base($"Conversion from type {sourceType.Name} to type {propertyType.Name} is not supported.") { }

        public NotSupportedPropertyConversionException(Type propertyType)
            : base($"Conversion to type {propertyType.Name} is not supported.") { }
    }
}
