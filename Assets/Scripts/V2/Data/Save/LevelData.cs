namespace Prez.V2.Data.Save
{
    public class LevelData
    {
        public int Level = 1;
        public double TimeThisLevel = 0d;
        public double TimeTotal = 0d;

        public readonly float ExperienceGainedPerBrickBase = 0.1f;
        public double Experience;
        public double ExperienceRequiredToLevel = 0d;
        
        public double GetExperienceForBrickDamage(DamageData data)
        {
            if (!data.BrickDestroyed)
                return 0;
        
            return ExperienceGainedPerBrickBase * data.Brick.RowId;
        }
        
        public double GetExperienceNeededToLevel(int level = 0)
        {
            if (level <= 0)
                level = Level;
            
            return Numbers.GetLevelExperience(level);
        }
    }
}