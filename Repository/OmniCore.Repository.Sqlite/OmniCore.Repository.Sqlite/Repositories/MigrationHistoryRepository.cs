﻿using OmniCore.Model.Interfaces.Data.Entities;
using OmniCore.Model.Interfaces.Data.Repositories;
using OmniCore.Model.Interfaces;
using OmniCore.Repository.Sqlite.Entities;

namespace OmniCore.Repository.Sqlite.Repositories
{
    public class MigrationHistoryRepository : Repository<MigrationHistoryEntity, IMigrationHistoryEntity>, IMigrationHistoryRepository
    {
        public MigrationHistoryRepository(IRepositoryService repositoryService) : base(repositoryService)
        {
        }
    }
}