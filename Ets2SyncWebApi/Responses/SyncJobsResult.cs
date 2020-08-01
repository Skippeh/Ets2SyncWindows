using System;
using System.IO;

namespace Ets2SyncWebApi.Responses
{
    public class SyncJobsResult
    {
        public enum SyncResult
        {
            Success,
            InternalServerError,
            UnknownError
        }
        
        public byte[] SaveFileBytes { get; set; }
        public SyncResult Result { get; set; }
        public Exception ErrorException { get; set; }
    }
}