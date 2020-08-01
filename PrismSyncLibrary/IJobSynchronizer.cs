using System;
using System.IO;
using System.Threading.Tasks;
using PrismLibrary;

namespace PrismSyncLibrary
{
    public interface IJobSynchronizer
    {
        Task<SyncResult> SyncJobsAsync(GameSave gameSave, GameModType modType, int selectedDlc);
    }

    public class SyncResult
    {
        public ResultType Result { get; set; }
        public Exception Exception { get; set; }
        public byte[] SaveBytes { get; set; }
        
        private SyncResult() { }

        public static SyncResult Success(byte[] saveBytes)
        {
            if (saveBytes == null)
                throw new ArgumentNullException(nameof(saveBytes));

            if (saveBytes.Length == 0)
                throw new ArgumentException("Save bytes length == 0.", nameof(saveBytes));

            return new SyncResult
            {
                Result = ResultType.Success,
                SaveBytes = saveBytes
            };
        }

        public static SyncResult Fail(Exception exception, ResultType resultType = ResultType.Failed)
        {
            if (resultType == ResultType.Success)
                throw new ArgumentException("Result type can't be Success when creating a failed sync result.");

            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            
            return new SyncResult
            {
                Result = resultType,
                Exception = exception
            };
        }
    }
}