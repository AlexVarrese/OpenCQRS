﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Domain;
using OpenCqrs.Store.EF.Entities.Factories;

namespace OpenCqrs.Store.EF.Stores
{
    /// <inheritdoc />
    public class AggregateStore : IAggregateStore
    {
        private readonly IDomainDbContextFactory _dbContextFactory;
        private readonly IAggregateEntityFactory _aggregateEntityFactory;

        public AggregateStore(IDomainDbContextFactory dbContextFactory, IAggregateEntityFactory aggregateEntityFactory)
        {
            _dbContextFactory = dbContextFactory;
            _aggregateEntityFactory = aggregateEntityFactory;         
        }

        /// <inheritdoc />
        public async Task SaveAggregateAsync<TAggregate>(Guid id) where TAggregate : IAggregateRoot
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var aggregateEntity = await dbContext.Aggregates.FirstOrDefaultAsync(x => x.Id == id);               
                if (aggregateEntity == null)
                {
                    var newAggregateEntity = _aggregateEntityFactory.CreateAggregate<TAggregate>(id);
                    await dbContext.Aggregates.AddAsync(newAggregateEntity);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        /// <inheritdoc />
        public void SaveAggregate<TAggregate>(Guid id) where TAggregate : IAggregateRoot
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var aggregateEntity = dbContext.Aggregates.FirstOrDefault(x => x.Id == id);
                if (aggregateEntity == null)
                {
                    var newAggregateEntity = _aggregateEntityFactory.CreateAggregate<TAggregate>(id);
                    dbContext.Aggregates.Add(newAggregateEntity);
                    dbContext.SaveChanges();
                }
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AggregateStoreModel>> GetAggregatesAsync()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return await dbContext.Aggregates.Select(x => new AggregateStoreModel
                {
                    Id = x.Id,
                    Type = x.Type
                }).ToListAsync();
            }
        }

        /// <inheritdoc />
        public IEnumerable<AggregateStoreModel> GetAggregates()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return dbContext.Aggregates.Select(x => new AggregateStoreModel
                {
                    Id = x.Id,
                    Type = x.Type
                }).ToList();
            }
        }
    }
}
