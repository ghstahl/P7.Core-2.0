namespace P7.HugoStore.Core
{
    public class BiggyConfig
    {
        public string Folder { get; set; }
        public virtual string Database { get; set; }
        public BiggyConfig()
        {
        }

        public BiggyConfig(string folder, string database = null)
        {
            if (!string.IsNullOrEmpty(database))
            {
                Database = database;
            }
            Folder = folder;
        }
        public BiggyConfig UsingFolder(string folder)
        {
            this.Folder = folder;
            return this;
        }
        public BiggyConfig UsingDatabase(string database)
        {
            this.Database = database;
            return this;
        }
    }
}