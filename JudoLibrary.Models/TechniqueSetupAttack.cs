namespace JudoLibrary.Models
{
    public class TechniqueSetupAttack
    {
        public string TechniqueId { get; set; }
        public Technique Technique { get; set; } 

        public string SetUpAttackId { get; set; }
        public Technique SetUpAttack { get; set; } // One Technique can have Many SetUpAttacks
    }
}