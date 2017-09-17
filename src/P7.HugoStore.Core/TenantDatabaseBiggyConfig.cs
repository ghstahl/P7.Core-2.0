using System;

namespace P7.HugoStore.Core
{
    public class TenantDatabaseBiggyConfig : BiggyConfig
    {

        public Guid TenantId { get; set; }

        public new virtual string Database
        {
            get { return TenantId.ToString(); }
            set { }
        }

        public static Guid GlobalTenantId
        {
            get
            {
                return Guid.Parse("00000000-0000-0000-0000-000000000000");
            }
        }


        public TenantDatabaseBiggyConfig()
            : base()
        {
        }

        public TenantDatabaseBiggyConfig(string folder, Guid tenantId)
            : base(folder)
        {
            TenantId = tenantId;
        }

        public TenantDatabaseBiggyConfig UsingTenantId(Guid tenantId)
        {
            this.TenantId = tenantId;
            return this;
        }
    }
}