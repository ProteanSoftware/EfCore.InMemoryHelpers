using System;
using ApprovalTests;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class UniqueIndexTests : TestBase
    {
        public UniqueIndexTests(ITestOutputHelper output)
            :
            base(output)
        { }

        [Fact]
        public void RespectsUniqueIndexOrder()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntityUnique {A = "a", B = "b"};
                var entity2 = new TestEntityUnique {A = "b", B = "a"};
                context.AddRange(entity1, entity2);
                context.SaveChanges();
            }
        }

        [Fact]
        public void RespectsUniqueIndexWithFilter()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntityUnique { A = "a", B = "b" };
                var entity2 = new TestEntityUnique { A = "b", B = "a" };
                var entity3 = new TestEntityUnique { A = "filtered", B = "a" };
                var entity4 = new TestEntityUnique { A = "filtered", B = "a" };
                context.AddRange(entity1, entity2, entity3, entity4);
                context.SaveChanges();
            }
        }

        [Fact]
        public void UniqueIndexThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntity
                {
                    Property = "filtered"
                };
                context.Add(entity1);
                var user2 = new TestEntity
                {
                    Property = "filtered"
                };
                context.Add(user2);
                var exception = Assert.Throws<Exception>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        [Fact]
        public void UniqueIndexAllowedDueToFilter()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntity
                {
                    Property = "duplicated"
                };
                context.Add(entity1);
                var user2 = new TestEntity
                {
                    Property = "duplicated"
                };
                context.Add(user2);
            }
        }


        [Fact]
        public void UniqueIndexEnumFilter()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntityEnumFilter
                {
                    Property = "prop",
                    FilterIndexEnum = FilterIndexEnum.B
                };
                context.Add(entity1);
                var user2 = new TestEntityEnumFilter
                {
                    Property = "prop",
                    FilterIndexEnum = FilterIndexEnum.B
                };
                context.Add(user2);
            }
        }

        [Fact]
        public void UniqueIndexEnumFilterThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntityEnumFilter
                {
                    Property = "prop",
                    FilterIndexEnum = FilterIndexEnum.A
                };
                context.Add(entity1);
                var user2 = new TestEntityEnumFilter
                {
                    Property = "prop",
                    FilterIndexEnum = FilterIndexEnum.A
                };
                context.Add(user2);
                var exception = Assert.Throws<Exception>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        public class TestEntityUnique
        {
            public int Id { get; set; }
            public string A { get; set; }
            public string B { get; set; }
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Property { get; set; }
        }

        public class TestEntityEnumFilter
        {
            public int Id { get; set; }
            public string Property { get; set; }
            public FilterIndexEnum? FilterIndexEnum { get; set; }
        }

        public enum FilterIndexEnum
        {
            A = 1,
            B = 2
        }

        private class TestDataContext : DbContext
        {
            public TestDataContext(DbContextOptions options)
                : base(options)
            { }

            public DbSet<TestEntity> TestEntities { get; set; }

            public DbSet<TestEntityUnique> TestEntityUnique { get; set; }

            public DbSet<TestEntityUnique> TestEntityUniqueInclusiveFilter { get; set; }

            public DbSet<TestEntityEnumFilter> TestEntityEnumFilter { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var testEntity = modelBuilder.Entity<TestEntity>();
                testEntity.Property(b => b.Property)
                    .IsRequired();

                testEntity.HasIndex(u => u.Property)
                    .IsUnique()
                    .HasFilter("[Property] = 'filtered'");


                var testEntityWithEnumFilter = modelBuilder.Entity<TestEntityEnumFilter>();
                testEntityWithEnumFilter.Property(b => b.Property)
                    .IsRequired();

                testEntityWithEnumFilter.HasIndex(u => u.Property)
                    .IsUnique()
                    .HasFilter("[FilterIndexEnum] = 1'");

                var testEntitySameTypes = modelBuilder.Entity<TestEntityUnique>();
                testEntitySameTypes.HasIndex(u => new {u.A, u.B})
                    .IsUnique()
                    .HasFilter("[A] <> 'filtered'");
            }
        }
    }
}