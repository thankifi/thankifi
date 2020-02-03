using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Core.Domain.Gratitude.Notification;
using TaaS.Core.Entity;
using TaaS.Infrastructure.Contract.Service;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Import.Command.ImportGratitudes
{
    public class ImportGratitudesCommandHandler : IRequestHandler<ImportGratitudesCommand, Unit>
    {
        protected readonly ILogger<ImportGratitudesCommandHandler> Logger;
        protected readonly IImporterService ImporterService;
        protected readonly TaaSDbContext Context;
        protected readonly IMediator Mediator;

        public ImportGratitudesCommandHandler(ILogger<ImportGratitudesCommandHandler> logger, IImporterService importerService, TaaSDbContext context, IMediator mediator)
        {
            Logger = logger;
            ImporterService = importerService;
            Context = context;
            Mediator = mediator;
        }

        public async Task<Unit> Handle(ImportGratitudesCommand request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

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
                    var tableName = Context.Model.FindEntityType(typeof(GratitudeCategory)).GetTableName();
                    await Context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);
                    
                    tableName = Context.Model.FindEntityType(typeof(Entity.Category)).GetTableName();
                    await Context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);
                    
                    tableName = Context.Model.FindEntityType(typeof(Entity.Gratitude)).GetTableName();
                    await Context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);

                    foreach (var category in categories)
                    {
                        await Context.Categories.AddAsync(new Entity.Category
                        {
                            Id = category.Id,
                            Title = category.Title
                        }, cancellationToken);
                    }

                    foreach (var gratitude in gratitudes)
                    {
                        var gratitudeEntity = new Entity.Gratitude
                        {
                            Id = gratitude.Id,
                            Language = gratitude.Language,
                            Text = gratitude.Text,
                            Type = Enum.Parse<GratitudeType>(gratitude.Type),
                            Categories = new List<GratitudeCategory>()
                        };

                        foreach (var category in gratitude.Categories)
                        {
                            gratitudeEntity.Categories.Add(new GratitudeCategory{
                                GratitudeId = gratitudeEntity.Id,
                                CategoryId = categories.Single(c => string.Equals(c.Title, category, StringComparison.CurrentCultureIgnoreCase)).Id
                            });
                        }
                        
                        await Context.Gratitudes.AddAsync(gratitudeEntity, cancellationToken);
                    }

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
    }
}