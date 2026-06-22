using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Infraestructura.Data
{
    public static class ValueConversionExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions();

        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : class, new()
        {
            ValueConverter<T, string> converter = new ValueConverter<T, string>
            (
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<T>(v, JsonOptions) ?? new T()
            );

            ValueComparer<T> comparer = new ValueComparer<T>
            (
                (l, r) => JsonSerializer.Serialize(l, JsonOptions) == JsonSerializer.Serialize(r, JsonOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
                v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonOptions), JsonOptions)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }

        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            ValueConverter<T, string> converter = new ValueConverter<T, string>
            (
                v => JsonSerializer.Serialize(new ValueData<T>() { Value = v }, JsonOptions),
                v => JsonSerializer.Deserialize<ValueData<T>>(v, JsonOptions).Value
            );

            ValueComparer<T> comparer = new ValueComparer<T>
            (
                (l, r) => JsonSerializer.Serialize(new ValueData<T>() { Value = l }, JsonOptions) ==
                          JsonSerializer.Serialize(new ValueData<T>() { Value = r }, JsonOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(new ValueData<T>() { Value = v }, JsonOptions).GetHashCode(),
                v => JsonSerializer.Deserialize<ValueData<T>>(
                    JsonSerializer.Serialize(new ValueData<T>() { Value = v }, JsonOptions), JsonOptions).Value
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }

        public class ValueData<TValue>
        {
            public TValue Value { get; set; }
        }
    }
}
