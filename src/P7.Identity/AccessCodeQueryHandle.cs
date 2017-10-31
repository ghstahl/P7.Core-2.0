namespace P7.Identity
{
    public class AccessCodeQueryHandle
    {
        public string Id { get; set; }
      
        public AccessCodeQueryHandle()
        {
        }

        public AccessCodeQueryHandle(AccessCodeQueryHandle doc)
        {
            this.Id = doc.Id;

        }

        public override bool Equals(object obj)
        {
            if (!(obj is AccessCodeQueryHandle other))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}