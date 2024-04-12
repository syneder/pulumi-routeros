using Pulumi.Experimental.Provider;
using Pulumi.Extensions.Provider.Exceptions;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Pulumi.Extensions.Provider
{
    public static class PropertiesExtensions
    {
        private static readonly MethodInfo _enumerableToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))!;
        private static readonly ConcurrentDictionary<Type, MethodInfo> _enumerableToArrayMethods = [];
        private static readonly ConcurrentDictionary<Type, Type> _enumerableTypes = [];
        private static readonly ConcurrentDictionary<Type, Type> _collectionTypes = [];
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> _collectionConstructors = [];

        public static T GetRequired<T>(this IDictionary<string, PropertyValue> properties, string propertyName)
        {
            var value = GetProperty(properties, propertyName).GetValue(typeof(T), propertyName, true);
            return (T)(value ?? throw new InvalidOperationException(
                $"The property {propertyName} is specified as required, " +
                $"but the property value is missing or empty in the properties."));
        }

        public static T? Get<T>(this IDictionary<string, PropertyValue> properties, string propertyName)
        {
            return (T?)GetProperty(properties, propertyName).GetValue(typeof(T), propertyName);
        }

        public static object? GetValue(this PropertyValue property, Type propertyType, string propertyName)
        {
            return property.GetValue(propertyType, propertyName, default);
        }

        private static PropertyValue GetProperty(this IDictionary<string, PropertyValue> properties, string propertyName)
        {
            return properties.TryGetValue(propertyName, out var propertyValue) ? propertyValue : PropertyValue.Null;
        }

        private static object? GetValue(this PropertyValue property, Type propertyType, string propertyName, bool required)
        {
            object? createDefault()
            {
                return !required && propertyType.IsValueType ? Activator.CreateInstance(propertyType) : default;
            }

            if (property == PropertyValue.Null)
            {
                return createDefault();
            }

            if (property.TryGetSecret(out var secretProperty))
            {
                return secretProperty.GetValue(propertyType, $"secret({propertyName})", required);
            }

            try
            {
                return property.Match(
                    createDefault,
                    value => ConvertValue(value, propertyType),
                    value => ConvertValue(value, propertyType),
                    value => ConvertValue(value, propertyType),
                    values => values.GetValues(propertyType, propertyName),
                    dictionary => throw CreateNotSupportedException(dictionary, propertyType),
                    asset => throw CreateNotSupportedException(asset, propertyType),
                    archive => throw CreateNotSupportedException(archive, propertyType),
                    property => property.GetValue(propertyType, propertyName),
                    resourceReference => throw CreateNotSupportedException(resourceReference, propertyType),
                    outputReference => throw CreateNotSupportedException(outputReference, propertyType),
                    () => throw new NotSupportedException(
                        $"Conversion from computed value to type {propertyType.Name} is not supported."));
            }
            catch (FormatException ex)
            {
                throw new PropertyConversionException(propertyType, propertyName, ex);
            }
        }

        private static object? ConvertValue<T>(T value, Type propertyType)
        {
            return propertyType == typeof(T) ? value : Convert.ChangeType(value, propertyType);
        }

        private static object? GetValues(this IEnumerable<PropertyValue> properties, Type propertyType, string propertyName)
        {
            if (propertyType.IsArray)
            {
                var propertyElementType = propertyType.GetElementType();
                if (propertyElementType != null)
                {
                    var enumerableType = _enumerableTypes.GetOrAdd(propertyElementType, MakeEnumerableType);
                    var enumerableToArrayMethod = _enumerableToArrayMethods.GetOrAdd(propertyElementType, _ =>
                    {
                        return _enumerableToArrayMethod.MakeGenericMethod(propertyElementType);
                    });

                    var values = GetValues(properties, enumerableType, propertyName);
                    return enumerableToArrayMethod.Invoke(null, [values]);
                }
            }

            if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                var propertyTypeArguments = propertyType.GetGenericArguments();
                if (propertyTypeArguments.Length == 1)
                {
                    var collectionType = _collectionTypes.GetOrAdd(propertyTypeArguments[0], MakeCollectionType);
                    var collectionConstructor = _collectionConstructors.GetOrAdd(collectionType, _ =>
                    {
                        return collectionType.GetConstructor([collectionType]) ?? throw new InvalidOperationException(
                            $"A collection constructor with an argument of type {collectionType.Name} could not be found.");
                    });

                    return collectionConstructor.Invoke(
                        [properties.Select((property, index) => property.GetValue(propertyType, $"{propertyName}[{index++}]"))]);
                }
            }

            throw new NotSupportedPropertyConversionException(typeof(IEnumerable<PropertyValue>), propertyType);
        }

        private static NotSupportedPropertyConversionException CreateNotSupportedException<T>(T _, Type propertyType)
        {
            return new NotSupportedPropertyConversionException(typeof(T), propertyType);
        }

        private static Type MakeEnumerableType(Type instanceType)
        {
            return typeof(IEnumerable<>).MakeGenericType(instanceType);
        }

        private static Type MakeCollectionType(Type instanceType)
        {
            return typeof(List<>).MakeGenericType(instanceType);
        }
    }
}
