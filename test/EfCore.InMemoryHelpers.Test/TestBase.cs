using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class TestBase
    {
        protected readonly ITestOutputHelper Output;

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
        }
    }

    public static class ChangeTrackerExtensions
    {
        public static void DetachAllEntities(this ChangeTracker changeTracker)
        {
            var changedEntriesCopy = changeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
            {
                entry.
            }
        }
    }
}