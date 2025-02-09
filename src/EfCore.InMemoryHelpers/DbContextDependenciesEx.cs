﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

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
            IAsyncQueryProvider queryProvider,
            IStateManager stateManager,
            IExceptionDetector exceptionDetector,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger,
            IDiagnosticsLogger<DbLoggerCategory.Infrastructure> infrastructureLogger)
        {
            inner = new DbContextDependencies(
                currentContext,
                changeDetector,
                setSource,
                entityFinderSource,
                entityGraphAttacher,
                queryProvider, 
                new StateManagerWrapper(stateManager),
                exceptionDetector,
                updateLogger,
                infrastructureLogger);
        }

        public IDbSetSource SetSource => inner.SetSource;

        public IEntityFinderFactory EntityFinderFactory => inner.EntityFinderFactory;

        public IAsyncQueryProvider QueryProvider => inner.QueryProvider;

        public IStateManager StateManager => inner.StateManager;

        public IChangeDetector ChangeDetector => inner.ChangeDetector;

        public IEntityGraphAttacher EntityGraphAttacher => inner.EntityGraphAttacher;

        public IExceptionDetector ExceptionDetector => inner.ExceptionDetector;

        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger => inner.UpdateLogger;

        public IDiagnosticsLogger<DbLoggerCategory.Infrastructure> InfrastructureLogger => inner.InfrastructureLogger;
    }
}