using System;

namespace OldSchool.RazorPages
{
    public class OldSchoolMagicService : IOldSchoolMagicService
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
