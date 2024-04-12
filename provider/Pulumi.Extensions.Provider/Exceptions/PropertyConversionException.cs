namespace Pulumi.Extensions.Provider.Exceptions
{
    public class PropertyConversionException(Type propertyType, string propertyName, FormatException? innerException)
        : Exception($"The operation to convert value to type {propertyType.Name} for the {propertyName} property is invalid.", innerException)
    {
        public PropertyConversionException(Type propertyType, string propertyName) : this(propertyType, propertyName, null) { }
    }
}
