namespace JudoLibrary.Api.Form
{
    public class UpdateTechniqueForm : CreateTechniqueForm
    {
        public int Id { get; set; }
        public string Reason { get; set; }
    }
}