using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7.BlogStore.Hugo.Extensions
{
    internal class MyBiggyConfiguration : IBlogStoreBiggyConfiguration
    {
        public string DatabaseName { get; set; }
        public string FolderStorage { get; set; }
        public string TenantId { get; set; }
    }
}
