using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

//TODO: remove when this is fixed https://github.com/aspnet/EntityFrameworkCore/issues/2166
namespace EfCore.InMemoryHelpers
{
    internal static class IndexValidator
    {
        public static void ValidateIndexes(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries().GroupBy(x => x.Metadata))
            {
                foreach (var index in entry.Key.UniqueIndices())
                {
                    index.ValidateEntities(entry.Select(x => x.Entity));
                }
            }
        }

        private static void ValidateEntities(this IIndex index, IEnumerable<object> entities)
        {
            var dictionary = new Dictionary<long, List<object>>();
            foreach (var entity in entities)
            {
                var filteredIndex = index.GetAnnotations().SingleOrDefault(x => x.Name == "Relational:Filter");
                if (filteredIndex?.Value is string filter)
                {
                    var parts = filter.Split(' ');

                    var column = parts[0].TrimStart('[').TrimEnd(']');

                    var filterProperty = entity.GetType().GetProperty(column);
                    var value = (object)parts[2].TrimStart('\'').TrimEnd('\'');

                    var type = filterProperty.PropertyType;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = filterProperty.PropertyType.GetGenericArguments().Single();
                    }

                    if (type.IsEnum)
                    {
                        var enumBaseType = type.GetEnumUnderlyingType();
                        value = Convert.ChangeType(value, enumBaseType);
                        value = Enum.ToObject(type, value);
                    }

                    value = Convert.ChangeType(value, type);

                    var entityValue = filterProperty.GetValue(entity);
                    var check = parts[1];

                    if (check == "=" && !value.Equals(entityValue))
                    {
                        continue;
                    }

                    if (check == "<>" && value.Equals(entityValue))
                    {
                        continue;
                    }
                }

                var valueLookup = index.GetProperties(entity).ToList();
                var values = valueLookup.Select(x => x.value).ToList();
                if (values.Any(x => x == null))
                {
                    continue;
                }

                var hash = values.GetHash();

                if (!dictionary.ContainsKey(hash))
                {
                    dictionary[hash] = values;
                    continue;
                }

                var builder = new StringBuilder($"Conflicting values for unique index. Entity: {entity.GetType().FullName},\r\nIndex Properties:\r\n");
                foreach ((var name, var value) in valueLookup)
                {
                    builder.AppendLine($"    {name}='{value}'");
                }

                throw new Exception(builder.ToString());
            }
        }

        private static IEnumerable<IIndex> UniqueIndices(this IEntityType entityType)
        {
            return entityType.GetIndexes()
                .Where(x => x.IsUnique);
        }

        private static long GetHash(this IEnumerable<object> values)
        {
            return string.Join("/", values).GetHashCode();
        }

        private static IEnumerable<(string name, object value)> GetProperties(this IIndex index, object entity)
        {
            return index.Properties
                .Select(property => property.PropertyInfo)
                .Select(info => (info.Name, info.GetValue(entity)));
        }
    }
}