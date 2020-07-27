namespace JudoLibrary.Models
{
    public class TechniqueFollowupAttack
    {
        public string TechniqueId { get; set; }
        public Technique Technique { get; set; } 

        public string FollowUpAttackId { get; set; }
        public Technique FollowUpAttack { get; set; } // One Technique can have Many FollowUpAttacks
    }
}