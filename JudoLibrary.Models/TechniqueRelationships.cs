namespace JudoLibrary.Models
{
    public class TechniqueRelationships
    {
        public int SetUpAttackId { get; set; }
        public Technique SetUpAttack { get; set; } // One Technique can have Many SetUpAttacks
        public int FollowUpAttackId { get; set; }
        public Technique FollowUpAttack { get; set; } // One Technique can have Many FollowUpAttacks
        public bool Active { get; set; }
    }
}