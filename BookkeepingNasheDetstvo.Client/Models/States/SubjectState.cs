using BookkeepingNasheDetstvo.Client.Models.ForSubject;

namespace BookkeepingNasheDetstvo.Client.Models.States
{
    internal sealed class SubjectState
    {
        public SelectedChildrenModel SelectedChildren { get; set; }

        public bool IsConsultation { get; set; }
    }
}
