using System;
using MediatR;

namespace TaaS.Core.Domain.Import.Notification
{
    public class ImportErrorNotification : INotification
    {
        public ImportErrorNotification(DateTime started, string fetchResultError)
        {
            Started = started;
            Error = fetchResultError;
            Finished = DateTime.UtcNow;
        }
        
        public string Error { get; }
        public DateTime Started { get; }
        public DateTime Finished { get; }
    }
}