﻿using System;
using Xunit;
using Xunit.Abstractions;
using ApprovalTests;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ConcurrencyWithLongTests : TestBase
{
    [Fact]
    public void NullConflictThrows()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            var update = new TestEntity
            {
                Id = entity.Id,
                Property = "Something new"
            };
            context.Entry(update).Property("Property").IsModified = true;
            var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
            Approvals.Verify(exception.Message.Replace(entity.Timestamp.ToString(),""));
        }
    }

    [Fact]
    public void UpdateSucceeds()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            var firstTimestamp = entity.Timestamp;
            var update = new TestEntity
            {
                Id = entity.Id,
                Property = "Something new",
                Timestamp = firstTimestamp
            };
            context.Entry(update).Property("Property").IsModified = true;
            context.SaveChanges();
            Assert.NotEqual(firstTimestamp, update.Timestamp);
        }
    }

    [Fact]
    public void ValueConflictThrows()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            var update = new TestEntity
            {
                Id = entity.Id,
                Property = "Something new",
                Timestamp = 1
            };
            context.Entry(update).Property("Property").IsModified = true;
            var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
            Approvals.Verify(exception.Message.Replace(entity.Timestamp.ToString(),""));
        }
    }

    [Fact]
    public void SetRowVersionToZeroThrows()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            entity.Timestamp = 0;
            var exception = Assert.Throws<Exception>(() => context.SaveChanges());
            Approvals.Verify(exception.Message);
        }
    }

    [Fact]
    public void UpdateSameEntity()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            entity.Property = "Something new";
            context.SaveChanges();
        }
    }

    [Fact]
    public void UpdateMultipleSameEntity()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity1 = new TestEntity
            {
                Property = "prop"
            };
            var entity2 = new TestEntity
            {
                Property = "prop"
            };
            context.AddRange(entity1, entity2);
            context.SaveChanges();
            entity1.Property = "Something new";
            entity2.Property = "Something new";
            context.SaveChanges();
        }
    }

    public ConcurrencyWithLongTests(ITestOutputHelper output) :
        base(output)
    {
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Property { get; set; }
        public ulong Timestamp { get; set; }
    }

    class TestDataContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        public TestDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<TestEntity>();
            entity.Property(p => p.Timestamp)
                .HasConversion(new NumberToBytesConverter<ulong>())
                .IsRowVersion()
                .IsRequired()
                .HasColumnType("RowVersion");
        }
    }
}