namespace JudoLibrary.Models
{
    public class TechniqueFollowupAttack
    {
        public int TechniqueId { get; set; }
        public Technique Technique { get; set; }
        
        public int FollowUpAttackId { get; set; }
        public Technique FollowUpAttack { get; set; } // One Technique can have Many FollowUpAttacks
        public bool Active { get; set; }
    }
}