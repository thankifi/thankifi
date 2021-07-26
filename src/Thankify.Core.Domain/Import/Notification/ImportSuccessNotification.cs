using System;
using MediatR;

namespace Thankify.Core.Domain.Import.Notification
{
    public class ImportSuccessNotification : INotification
    {
        public ImportSuccessNotification(DateTime started)
        {
            Started = started;
            Finished = DateTime.UtcNow;
        }

        public DateTime Started { get; }
        public DateTime Finished { get; }
    }
}