using System.Collections.Generic;

namespace BookkeepingNasheDetstvo.Client.Models.Subject
{
    public sealed class SelectedChildrenModel
    {
        public List<SubjectItemModel> SelectedChildren { get; set; }
        
        public string SubjectId { get; set; }
    }
}