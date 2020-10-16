namespace JudoLibrary.Models
{
    public class TechniqueSetupAttack
    {
        public int TechniqueId { get; set; }
        public Technique Technique { get; set; } 

        public int SetUpAttackId { get; set; }
        public Technique SetUpAttack { get; set; } // One Technique can have Many SetUpAttacks
    }
}