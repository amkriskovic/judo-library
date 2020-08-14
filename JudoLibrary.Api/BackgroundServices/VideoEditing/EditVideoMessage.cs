namespace JudoLibrary.Api.BackgroundServices.VideoEditing
{
    // Class that is going to be passed between processes, with props: SubmissionId & Input
    public class EditVideoMessage
    {
        public int SubmissionId { get; set; }
        public string Input { get; set; }
    }
}