using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankifi.Common.Importer.Abstractions;
using Thankifi.Core.Application.Entity;
using Thankifi.Core.Entity;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Application.Import
{
    public class ImportService
    {
        private readonly ILogger<ImportService> _logger;
        private readonly IImporter _importer;
        private readonly ThankifiDbContext _dbContext;

        public ImportService(ILogger<ImportService> logger, IImporter importer, ThankifiDbContext dbContext)
        {
            _logger = logger;
            _importer = importer;
            _dbContext = dbContext;
        }

        public async Task TryImport(CancellationToken cancellationToken = default)
        {
            var applicationState = await _dbContext.ApplicationState.SingleOrDefaultAsync(cancellationToken);

            if (applicationState is null)
            {
                applicationState = new ApplicationState
                {
                    DatasetVersion = 0,
                    LastUpdated = DateTime.UtcNow
                };

                _logger.LogInformation("Generating new application state: {@ApplicationState}", applicationState);
                
                await _dbContext.ApplicationState.AddAsync(applicationState, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var datasetVersion = await _importer.GetVersion(cancellationToken);

            if (datasetVersion is null)
            {
                _logger.LogWarning("Import attempt aborted because dataset version was null");
                return;
            }

            if (datasetVersion <= applicationState.DatasetVersion)
            {
                _logger.LogInformation("Import attempt aborted because fetched version is older or equal to local version");
                return;
            }
            
            var dataset = await _importer.GetDataset(cancellationToken);

            if (dataset is null)
            {
                _logger.LogWarning("Import attempt aborted because a newer version ({DatasetVersion}) was fetched but the dataset was empty", datasetVersion);
                return;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogInformation("Importing dataset within transaction with id {TransactionId}", transaction.TransactionId);

                await CleanTablesAsync(cancellationToken);

                await AddOrUpdateCategoriesAsync(dataset.Categories, cancellationToken);
                await AddOrUpdateLanguagesAsync(dataset.Languages, cancellationToken);
                await AddOrUpdateGratitudesAsync(dataset.Gratitudes, cancellationToken);
                await UpdateApplicationStateAsync(datasetVersion, cancellationToken);

                _logger.LogInformation("Saving changes to database within transaction with id {TransactionId}", transaction.TransactionId);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled error during import database transaction");
            }
            finally
            {
                _logger.LogWarning("Rolling back database transaction with id {TransactionId} due to unhandled error during import", transaction.TransactionId);
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        private async Task CleanTablesAsync(CancellationToken cancellationToken = default)
        {
            var tableNames = _dbContext.Model.GetEntityTypes().Select(type => type.GetTableName());

            foreach (var tableName in tableNames)
            {
                _logger.LogInformation("Cleaning {TableName} table during transaction", tableName);
                await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\"", cancellationToken);
            }
        }

        private async Task AddOrUpdateCategoriesAsync(IEnumerable<Dataset.Category>? categories, CancellationToken cancellationToken = default)
        {
            foreach (var categoryToImport in categories ?? Array.Empty<Dataset.Category>())
            {
                var category = await _dbContext.Categories.FirstOrDefaultAsync(e => e.Id == categoryToImport.Id, cancellationToken);

                if (category is null)
                {
                    category = new Category
                    {
                        Id = categoryToImport.Id,
                        Slug = categoryToImport.Slug
                    };

                    _logger.LogInformation("Adding new category {@Category}", category);
                    await _dbContext.AddAsync(category, cancellationToken);
                }
                else if (category.Slug == categoryToImport.Slug)
                {
                    _logger.LogInformation("Updating category slug for category with id {CategoryId}", category.Id);
                    category.Slug = categoryToImport.Slug;
                }
            }
        }

        private async Task AddOrUpdateLanguagesAsync(IEnumerable<Dataset.Language>? languages, CancellationToken cancellationToken = default)
        {
            foreach (var languageToImport in languages ?? Array.Empty<Dataset.Language>())
            {
                var language = await _dbContext.Languages.FirstOrDefaultAsync(e => e.Id == languageToImport.Id, cancellationToken);

                if (language is null)
                {
                    language = new Language
                    {
                        Id = languageToImport.Id,
                        Code = languageToImport.Code
                    };

                    _logger.LogInformation("Adding new language {@Language}", language);
                    await _dbContext.AddAsync(language, cancellationToken);
                }
                else if (language.Code != languageToImport.Code)
                {
                    _logger.LogInformation("Updating language code for language with id {LanguageId}", language.Id);
                    language.Code = languageToImport.Code;
                }
            }
        }

        private async Task AddOrUpdateGratitudesAsync(IEnumerable<Dataset.Gratitude>? gratitudes, CancellationToken cancellationToken = default)
        {
            foreach (var gratitudeToImport in gratitudes ?? Array.Empty<Dataset.Gratitude>())
            {
                var gratitude = await _dbContext.Gratitudes.FirstOrDefaultAsync(e => e.Id == gratitudeToImport.Id, cancellationToken);

                if (gratitude is null)
                {
                    var language =  await GetLanguageOrDefault(gratitudeToImport.Language);
                    var categories = await GetCategoriesOrDefault(gratitudeToImport.Categories);

                    if (language is null || categories.Count != gratitudeToImport.Categories.Count)
                    {
                        _logger.LogWarning("Skipping import of gratitude with id {GratitudeId} because of language or categories mismatch", gratitudeToImport.Id);
                        continue;
                    }

                    await AddGratitude(gratitudeToImport.Id, gratitudeToImport.Text, language, categories);
                }
                else
                {
                    if (gratitude.Text != gratitudeToImport.Text)
                    {
                        _logger.LogInformation("Updating text for gratitude with id {GratitudeId}", gratitude.Id);
                        gratitude.Text = gratitudeToImport.Text;
                    }
                    
                    if (gratitude.Language.Code != gratitudeToImport.Language)
                    {
                        var language =  await GetLanguageOrDefault(gratitudeToImport.Language);

                        if (language is null)
                        {
                            _logger.LogWarning("Skipping update of gratitude with id {GratitudeId} because of language mismatch", gratitudeToImport.Id);
                            continue;
                        }

                        _logger.LogInformation("Updating language for gratitude with id {GratitudeId}", gratitude.Id);
                        _logger.LogInformation("New language for gratitude with id {GratitudeId}: {@Language}", gratitude.Id, gratitude.Language);
                        gratitude.Language = language;
                    }

                    if (!gratitude.Categories.Select(e => e.Slug).OrderBy(e => e).SequenceEqual(gratitudeToImport.Categories.OrderBy(e => e)))
                    {
                        var categories = await GetCategoriesOrDefault(gratitudeToImport.Categories);
                        
                        if (categories.Count != gratitudeToImport.Categories.Count)
                        {
                            _logger.LogWarning("Skipping update of gratitude with id {GratitudeId} because of categories mismatch", gratitudeToImport.Id);
                            continue;
                        }

                        _logger.LogInformation("Updating categories for gratitude with id {GratitudeId}", gratitude.Id);
                        _logger.LogInformation("New categories for gratitude with id {GratitudeId}: {@Categories}", gratitude.Id, gratitude.Categories);
                        gratitude.Categories = categories;
                    }
                }
            }

            async Task<Language?> GetLanguageOrDefault(string code)
            {
                return await _dbContext.Languages.FirstOrDefaultAsync(e => e.Code == code, cancellationToken);
            }
            
            async Task<List<Category>> GetCategoriesOrDefault(IEnumerable<string> slugs)
            {
                return await _dbContext.Categories.Where(e => slugs.Contains(e.Slug)).ToListAsync(cancellationToken);
            }

            async Task AddGratitude(Guid id, string text, Language? language, List<Category>? categories)
            {
                var gratitude = new Gratitude
                {
                    Id = id,
                    Text = text,
                    Language = language,
                    Categories = categories
                };

                _logger.LogInformation("Adding new gratitude {@Gratitude}", gratitude);
                await _dbContext.AddAsync(gratitude, cancellationToken);
            }
        }
        
        private async Task UpdateApplicationStateAsync(int? datasetVersion, CancellationToken cancellationToken = default)
        {
            var applicationState = await _dbContext.ApplicationState.SingleOrDefaultAsync(cancellationToken);
            
            applicationState.DatasetVersion = datasetVersion ?? throw new ArgumentNullException(nameof(datasetVersion));
            applicationState.LastUpdated = DateTime.UtcNow;
            
            _logger.LogInformation("Updating application dataset version to {DatasetVersion}", applicationState.DatasetVersion);
        }
    }
}