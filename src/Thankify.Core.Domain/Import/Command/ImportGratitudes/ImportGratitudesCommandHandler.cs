using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankify.Core.Domain.Import.Notification;
using Thankify.Core.Entity;
using Thankify.Infrastructure.Contract.Service;
using Thankify.Persistence.Context;

namespace Thankify.Core.Domain.Import.Command.ImportGratitudes
{
    public class ImportGratitudesCommandHandler : IRequestHandler<ImportGratitudesCommand, Unit>
    {
        protected readonly ILogger<ImportGratitudesCommandHandler> Logger;
        protected readonly IImporterService ImporterService;
        protected readonly ThankifyDbContext Context;
        protected readonly IMediator Mediator;

        public ImportGratitudesCommandHandler(ILogger<ImportGratitudesCommandHandler> logger, IImporterService importerService, ThankifyDbContext context, IMediator mediator)
        {
            Logger = logger;
            ImporterService = importerService;
            Context = context;
            Mediator = mediator;
        }

        public async Task<Unit> Handle(ImportGratitudesCommand request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

            var version = await Context.Version.SingleOrDefaultAsync(cancellationToken);

            var versionFetchResult = await ImporterService.FindCurrentVersion(cancellationToken);
            if (versionFetchResult.IsError)
            {
                await Mediator.Publish(new ImportErrorNotification(startTime, versionFetchResult.Error), cancellationToken);
                return Unit.Value;
            }

            if (version?.Version == versionFetchResult.Value)
            {
                await Mediator.Publish(new ImportSuccessNotification(startTime), cancellationToken);
                return Unit.Value;
            }
            
            var fetchResult = await ImporterService.Fetch(cancellationToken);
            if (fetchResult.IsError)
            {
                await Mediator.Publish(new ImportErrorNotification(startTime, fetchResult.Error), cancellationToken);
                return Unit.Value;
            }

            var (gratitudes, categories) = fetchResult.Value;
            
            await using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    await CleanTablesAsync(cancellationToken);

                    await InsertCategoriesAsync(categories, cancellationToken);

                    await InsertGratitudesAsync(gratitudes, categories, cancellationToken);

                    await AddOrUpdateVersionAsync(version, versionFetchResult.Value, cancellationToken);

                    await Context.SaveChangesAsync(cancellationToken);
                    
                    await transaction.CommitAsync(cancellationToken);

                    await Mediator.Publish(new ImportSuccessNotification(startTime), cancellationToken);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Unhandled error while updating database.");
                    await Mediator.Publish(new ImportErrorNotification(startTime, e.Message), cancellationToken);
                }
            }
            
            return Unit.Value;
        }

        private async Task AddOrUpdateVersionAsync(ImportVersion? version, string versionFetchResult, CancellationToken cancellationToken)
        {
            if (version != null)
            {
                version.Version = versionFetchResult;
            }
            else
            {
                await Context.Version.AddAsync(new ImportVersion
                {
                    Id = Guid.NewGuid(),
                    Version = versionFetchResult
                }, cancellationToken);
            }
        }

        private async Task InsertGratitudesAsync(IEnumerable<Infrastructure.Contract.Model.Gratitude> gratitudes,
            IReadOnlyCollection<Infrastructure.Contract.Model.Category> categories, 
            CancellationToken cancellationToken)
        {
            foreach (var gratitude in gratitudes)
            {
                var gratitudeEntity = new Entity.Gratitude
                {
                    Id = gratitude.Id,
                    Language = gratitude.Language,
                    Text = gratitude.Text,
                    Categories = new List<GratitudeCategory>()
                };

                foreach (var category in gratitude.Categories)
                {
                    gratitudeEntity.Categories.Add(new GratitudeCategory
                    {
                        GratitudeId = gratitudeEntity.Id,
                        CategoryId = categories.Single(c => string.Equals(c.Title, category, StringComparison.CurrentCultureIgnoreCase)).Id
                    });
                }

                await Context.Gratitudes.AddAsync(gratitudeEntity, cancellationToken);
            }
        }

        private async Task InsertCategoriesAsync(IEnumerable<Infrastructure.Contract.Model.Category> categories, CancellationToken cancellationToken)
        {
            foreach (var category in categories)
            {
                await Context.Categories.AddAsync(new Entity.Category
                {
                    Id = category.Id,
                    Title = category.Title
                }, cancellationToken);
            }
        }

        private async Task CleanTablesAsync(CancellationToken cancellationToken)
        {
            var tableName = Context.Model.FindEntityType(typeof(GratitudeCategory)).GetTableName();
            await Context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);

            tableName = Context.Model.FindEntityType(typeof(Entity.Category)).GetTableName();
            await Context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);

            tableName = Context.Model.FindEntityType(typeof(Entity.Gratitude)).GetTableName();
            await Context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);
        }
    }
}