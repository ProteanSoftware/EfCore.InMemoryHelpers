using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EfCore.InMemoryHelpers
{
    internal class StateManagerWrapper : IStateManager
    {
        private readonly IStateManager inner;
        private readonly ConcurrencyValidator concurrencyValidator;

        public StateManagerWrapper(IStateManager stateManager)
        {
            inner = stateManager;
            concurrencyValidator = new ConcurrencyValidator();
        }

        public IList<IUpdateEntry> GetEntriesToSave(bool cascadeChanges)
        {
            return inner.GetEntriesToSave(cascadeChanges);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            inner.Context.ValidateIndexes();
            concurrencyValidator.ValidateIndexes(inner.Context);
            return inner.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellation = default)
        {
            inner.Context.ValidateIndexes();
            concurrencyValidator.ValidateIndexes(inner.Context);
            return inner.SaveChangesAsync(acceptAllChangesOnSuccess, cancellation);
        }

        public void ResetState()
        {
            inner.ResetState();
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return inner.ResetStateAsync(cancellationToken);
        }

        public InternalEntityEntry GetOrCreateEntry(object entity)
        {
            return inner.GetOrCreateEntry(entity);
        }

        public InternalEntityEntry GetOrCreateEntry(object entity, IEntityType entityType)
        {
            return inner.GetOrCreateEntry(entity, entityType);
        }

        public InternalEntityEntry CreateEntry(IDictionary<string, object> values, IEntityType entityType)
        {
            return inner.CreateEntry(values, entityType);
        }

        public InternalEntityEntry StartTrackingFromQuery(IEntityType baseEntityType, object entity, in ValueBuffer valueBuffer)
        {
            return inner.StartTrackingFromQuery(baseEntityType, entity, in valueBuffer);
        }


        public InternalEntityEntry TryGetEntry(IKey key, object[] keyValues)
        {
            return inner.TryGetEntry(key, keyValues);
        }

        public InternalEntityEntry? TryGetEntry(IKey key, object?[] keyValues, bool throwOnNullKey, out bool hasNullKey)
        {
            return inner.TryGetEntry(key, keyValues, throwOnNullKey, out hasNullKey);
        }

        public InternalEntityEntry TryGetEntry(object entity, bool throwOnNonUniqueness = true)
        {
            return inner.TryGetEntry(entity, throwOnNonUniqueness);
        }

        public InternalEntityEntry? TryGetEntry(object entity, IEntityType type, bool throwOnTypeMismatch = true)
        {
            return inner.TryGetEntry(entity, type, throwOnTypeMismatch);
        }

        public IEnumerable<InternalEntityEntry> GetEntriesForState(bool added = false, bool modified = false, bool deleted = false, bool unchanged = false, bool returnSharedIdentity = false)
        {
            return inner.GetEntriesForState(added, modified, deleted, unchanged, returnSharedIdentity);
        }

        public int GetCountForState(bool added = false, bool modified = false, bool deleted = false, bool unchanged = false, bool returnSharedIdentity = false)
        {
            return inner.GetCountForState(added, modified, deleted, unchanged, returnSharedIdentity);
        }

        public IEnumerable<TEntity> GetNonDeletedEntities<TEntity>() where TEntity : class
        {
            return inner.GetNonDeletedEntities<TEntity>();
        }

        public void ChangingState(InternalEntityEntry entry, EntityState newState)
        {
            inner.ChangingState(entry, newState);
        }

        public InternalEntityEntry StartTracking(InternalEntityEntry entry)
        {
            return inner.StartTracking(entry);
        }

        public void StopTracking(InternalEntityEntry entry, EntityState oldState)
        {
            inner.StopTracking(entry, oldState);
        }

        public void RecordReferencedUntrackedEntity(object referencedEntity, INavigationBase navigation, InternalEntityEntry referencedFromEntry)
        {
            inner.RecordReferencedUntrackedEntity(referencedEntity, navigation, referencedFromEntry);
        }

        public void UpdateReferencedUntrackedEntity(object referencedEntity, object newReferencedEntity, INavigationBase navigation, InternalEntityEntry referencedFromEntry)
        {
            inner.UpdateReferencedUntrackedEntity(referencedEntity, newReferencedEntity, navigation, referencedFromEntry);
        }

        public bool ResolveToExistingEntry(InternalEntityEntry newEntry, INavigationBase navigation, InternalEntityEntry referencedFromEntry)
        {
            return inner.ResolveToExistingEntry(newEntry, navigation, referencedFromEntry);
        }

        IEnumerable<Tuple<INavigationBase, InternalEntityEntry>> IStateManager.GetRecordedReferrers(object referencedEntity, bool clear)
        {
            return inner.GetRecordedReferrers(referencedEntity, clear);
        }

        public void BeginAttachGraph()
        {
            inner.BeginAttachGraph();
        }

        public void CompleteAttachGraph()
        {
            inner.CompleteAttachGraph();
        }

        public void AbortAttachGraph()
        {
            inner.AbortAttachGraph();
        }

        public InternalEntityEntry? FindPrincipal(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return inner.FindPrincipal(dependentEntry, foreignKey);
        }

        public InternalEntityEntry? FindPrincipalUsingPreStoreGeneratedValues(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return FindPrincipalUsingPreStoreGeneratedValues(dependentEntry, foreignKey);
        }

        public InternalEntityEntry? FindPrincipalUsingRelationshipSnapshot(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return inner.FindPrincipalUsingRelationshipSnapshot(dependentEntry, foreignKey);
        }

        public void UpdateIdentityMap(InternalEntityEntry entry, IKey principalKey)
        {
            inner.UpdateIdentityMap(entry, principalKey);
        }

        public void UpdateDependentMap(InternalEntityEntry entry, IForeignKey foreignKey)
        {
            inner.UpdateDependentMap(entry, foreignKey);
        }

        public IEnumerable<IUpdateEntry>? GetDependentsFromNavigation(IUpdateEntry principalEntry, IForeignKey foreignKey)
        {
            return inner.GetDependentsFromNavigation(principalEntry, foreignKey);
        }

        public IEnumerable<IUpdateEntry> GetDependents(IUpdateEntry principalEntry, IForeignKey foreignKey)
        {
            return inner.GetDependents(principalEntry, foreignKey);
        }

        public IEnumerable<IUpdateEntry> GetDependentsUsingRelationshipSnapshot(IUpdateEntry principalEntry, IForeignKey foreignKey)
        {
            return inner.GetDependentsUsingRelationshipSnapshot(principalEntry, foreignKey);
        }

        public void AcceptAllChanges()
        {
            inner.AcceptAllChanges();
        }

        public IEntityFinder CreateEntityFinder(IEntityType entityType)
        {
            return inner.CreateEntityFinder(entityType);
        }

        public void Unsubscribe()
        {
            inner.Unsubscribe();
        }

        public (EventHandler<EntityTrackingEventArgs> Tracking, EventHandler<EntityTrackedEventArgs> Tracked, EventHandler<EntityStateChangingEventArgs> StateChanging, EventHandler<EntityStateChangedEventArgs> StateChanged) CaptureEvents()
        {
            return CaptureEvents();
        }

        public void SetEvents(EventHandler<EntityTrackingEventArgs> tracking, EventHandler<EntityTrackedEventArgs> tracked, EventHandler<EntityStateChangingEventArgs> stateChanging, EventHandler<EntityStateChangedEventArgs> stateChanged)
        {
            SetEvents(tracking, tracked, stateChanging, stateChanged);
        }

        public void OnTracking(InternalEntityEntry internalEntityEntry, EntityState state, bool fromQuery)
        {
            OnTracking(internalEntityEntry, state, fromQuery);
        }

        public void OnTracked(InternalEntityEntry internalEntityEntry, bool fromQuery)
        {
            inner.OnTracked(internalEntityEntry, fromQuery);
        }

        public void OnStateChanging(InternalEntityEntry internalEntityEntry, EntityState newState)
        {
            inner.OnStateChanging(internalEntityEntry, newState);
        }

        public void OnStateChanged(InternalEntityEntry internalEntityEntry, EntityState oldState)
        {
            inner.OnStateChanged(internalEntityEntry, oldState);
        }

        public void CascadeChanges(bool force)
        {
            inner.CascadeChanges(force);
        }

        public void CascadeDelete(InternalEntityEntry entry, bool force, IEnumerable<IForeignKey>? foreignKeys = null)
        {
            inner.CascadeDelete(entry, force, foreignKeys);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public StateManagerDependencies Dependencies => inner.Dependencies;

        public CascadeTiming DeleteOrphansTiming
        {
            get => inner.DeleteOrphansTiming;
            set => inner.DeleteOrphansTiming = value;
        }

        public CascadeTiming CascadeDeleteTiming
        {
            get => inner.CascadeDeleteTiming;
            set => inner.CascadeDeleteTiming = value;
        }

        public bool SavingChanges => inner.SavingChanges;

        public IEnumerable<InternalEntityEntry> Entries => inner.Entries;

        public int Count => inner.Count;

        public int ChangedCount
        {
            get => inner.ChangedCount;
            set => inner.ChangedCount = value;
        }

        public IInternalEntityEntryNotifier InternalEntityEntryNotifier => inner.InternalEntityEntryNotifier;
        public IValueGenerationManager ValueGenerationManager => inner.ValueGenerationManager;
        public DbContext Context => inner.Context;
        public IModel Model => inner.Model;
        IEntityMaterializerSource IStateManager.EntityMaterializerSource => EntityMaterializerSource;

        public IEntityMaterializerSource EntityMaterializerSource => inner.EntityMaterializerSource;
        public bool SensitiveLoggingEnabled => inner.SensitiveLoggingEnabled;
        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger => inner.UpdateLogger;
        public event EventHandler<EntityTrackingEventArgs> Tracking;

        public event EventHandler<EntityTrackedEventArgs> Tracked
        {
            add => inner.Tracked += value;
            remove => inner.Tracked -= value;
        }

        public event EventHandler<EntityStateChangingEventArgs> StateChanging
        {
            add => inner.StateChanging += value;
            remove => inner.StateChanging -= value;
        }

        public event EventHandler<EntityStateChangedEventArgs> StateChanged
        {
            add => inner.StateChanged += value;
            remove => inner.StateChanged -= value;
        }
    }
}