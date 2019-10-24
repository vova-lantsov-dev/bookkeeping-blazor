using System.Collections.Generic;

namespace BookkeepingNasheDetstvo.Client.Models.ForSubject
{
    public sealed class SelectedChildrenModel
    {
        public List<SubjectChildModel> SelectedChildren { get; set; }
        
        public string SubjectId { get; set; }
    }
}