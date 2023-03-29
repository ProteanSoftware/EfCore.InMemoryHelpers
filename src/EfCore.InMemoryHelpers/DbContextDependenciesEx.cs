﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EfCore.InMemoryHelpers
{
    internal class DbContextDependenciesEx : IDbContextDependencies
    {
        private readonly DbContextDependencies inner;

        public DbContextDependenciesEx(
            ICurrentDbContext currentContext,
            IChangeDetector changeDetector,
            IDbSetSource setSource,
            IEntityFinderSource entityFinderSource,
            IEntityGraphAttacher entityGraphAttacher,
            IModel model,
            IAsyncQueryProvider queryProvider,
            IStateManager stateManager,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger,
            IDiagnosticsLogger<DbLoggerCategory.Infrastructure> infrastructureLogger)
        {
            inner = new DbContextDependencies(
                currentContext,
                changeDetector,
                setSource,
                entityFinderSource,
                entityGraphAttacher,
                model,
                queryProvider, 
                new StateManagerWrapper(stateManager), updateLogger, infrastructureLogger);
        }

        public IModel Model => inner.Model;

        public IDbSetSource SetSource => inner.SetSource;

        public IEntityFinderFactory EntityFinderFactory => inner.EntityFinderFactory;

        public IAsyncQueryProvider QueryProvider => inner.QueryProvider;

        public IStateManager StateManager => inner.StateManager;

        public IChangeDetector ChangeDetector => inner.ChangeDetector;

        public IEntityGraphAttacher EntityGraphAttacher => inner.EntityGraphAttacher;

        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger => inner.UpdateLogger;

        public IDiagnosticsLogger<DbLoggerCategory.Infrastructure> InfrastructureLogger => inner.InfrastructureLogger;
    }
}