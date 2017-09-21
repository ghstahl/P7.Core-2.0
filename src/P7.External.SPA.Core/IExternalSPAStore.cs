using System;
using System.Collections.Generic;
using System.Text;

namespace P7.External.SPA.Core
{
    public interface IExternalSPAStore
    {
        ExternalSPARecord GetRecord(string key);
        void AddRecord(ExternalSPARecord record);
        void AddRecords(ExternalSPARecord[] record);
        
        void RemoveRecord(string key);
        IEnumerable<ExternalSPARecord> GetRecords();
    }
}
