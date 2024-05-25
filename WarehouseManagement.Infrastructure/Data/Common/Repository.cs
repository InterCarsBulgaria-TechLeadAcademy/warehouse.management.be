using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Common
{
    public class Repository : IRepository
    {
        private readonly WarehouseManagementDbContext context;

        /// <summary>
        /// Initializes a new instance of the Repository class with the specified database context.
        /// </summary>
        /// <param name="context">The database context to be used.</param>
        public Repository(WarehouseManagementDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add to the database.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync<T>(T entity)
            where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        /// <summary>
        /// Retrieves all records of type T from the database.
        /// </summary>
        /// <returns>An IQueryable representing the queryable expression tree.</returns>
        public IQueryable<T> All<T>()
            where T : class
        {
            return DbSet<T>();
        }

        /// <summary>
        /// Retrieves all records of type T from the database without tracking changes.
        /// </summary>
        /// <returns>An IQueryable representing the queryable expression tree without change tracking.</returns>
        public IQueryable<T> AllReadOnly<T>()
            where T : class
        {
            return DbSet<T>().AsNoTracking();
        }

        /// <summary>
        /// Retrieves all records of type T, including soft-deleted entities, from the database.
        /// </summary>
        /// <returns>An IQueryable representing the queryable expression tree.</returns>
        public IQueryable<T> AllWithDeleted<T>()
            where T : BaseClass
        {
            return DbSet<T>().IgnoreQueryFilters();
        }

        /// <summary>
        /// Retrieves all records of type T, including soft-deleted entities, from the database without tracking changes.
        /// </summary>
        /// <returns>An IQueryable representing the queryable expression tree without change tracking.</returns>
        public IQueryable<T> AllWithDeletedReadOnly<T>()
            where T : BaseClass
        {
            return DbSet<T>().IgnoreQueryFilters().AsNoTracking();
        }

        /// <summary>
        /// Asynchronously retrieves an entity of type T from the database based on the specified id.
        /// </summary>
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <returns>Returns the retrieved entity or null if not found.</returns>
        public async Task<T?> GetByIdAsync<T>(object id)
            where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        /// <summary>
        /// Asynchronously retrieves an entity of type T, including soft-deleted entities, from the database based on the specified id.
        /// </summary>
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <returns>Returns the retrieved entity or null if not found.</returns>
        public async Task<T?> GetByIdWithDeletedAsync<T>(int id)
            where T : BaseClass
        {
            return await DbSet<T>().IgnoreQueryFilters().FirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// Deletes the specified entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete from the database.</param>
        public void Delete<T>(T entity)
            where T : class
        {
            DbSet<T>().Remove(entity);
        }

        /// <summary>
        /// Retrieves a DbSet for the specified entity type from the database context.
        /// </summary>
        /// <returns>The DbSet instance for the specified entity type.</returns>
        private DbSet<T> DbSet<T>()
            where T : class
        {
            return context.Set<T>();
        }

        /// <summary>
        /// Asynchronously saves all changes made in the database context.
        /// </summary>
        /// <returns>Returns the number of affected rows.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the entity with the specified id from the database.
        /// </summary>
        /// <param name="id">The id of the entity to delete from the database.</param>
        public async Task DeleteById<T>(object id)
            where T : class
        {
            var entity = await GetByIdAsync<T>(id);
            if (entity != null)
            {
                DbSet<T>().Remove(entity);
            }
        }

        /// <summary>
        /// Deletes multiple entities from the database.
        /// </summary>
        /// <typeparam name="T">The type of entities to delete.</typeparam>
        /// <param name="entities">The list of entities to delete.</param>
        public void DeleteRange<T>(IEnumerable<T> entities)
            where T : class
        {
            DbSet<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Soft deletes the specified entity from the database.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        public void SoftDelete<T>(T entity)
            where T : BaseClass
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Asynchronously soft deletes the entity with the specified id from the database.
        /// </summary>
        /// <param name="id">The id of the entity to soft delete.</param>
        public async Task SoftDeleteById<T>(object id)
            where T : BaseClass
        {
            var entity = await GetByIdAsync<T>(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Restores a soft deleted entity in the database.
        /// </summary>
        /// <param name="entity">The entity to restore.</param>
        public void UnDelete<T>(T entity)
            where T : BaseClass
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedByUserId = null;
        }

        /// <summary>
        /// Asynchronously saves all changes made in the database context, along with logging the changes.
        /// </summary>
        public async Task<int> SaveChangesWithLogAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await context.SaveChangesWithLogAsync();
        }
    }
}
