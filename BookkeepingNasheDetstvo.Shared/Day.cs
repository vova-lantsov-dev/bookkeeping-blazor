using System.Collections.Generic;

namespace BookkeepingNasheDetstvo.Shared
{
    public class Day
    {
        public List<IdNamePair> Teachers { get; set; }
        public List<IdNamePair> Children { get; set; }
        public List<Subject> Subjects { get; set; }
    }
}
