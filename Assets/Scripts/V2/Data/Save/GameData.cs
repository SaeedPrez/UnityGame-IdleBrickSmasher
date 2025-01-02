namespace Prez.V2.Data.Save
{
    public class GameData
    {
        public string Id;
        public SettingsData SettingsData;
        public PaddleData PaddleData;
        public LevelData LevelData;
        public BricksData BricksData;
        
        /// <summary>
        /// Resets all game data.
        /// </summary>
        public void Reset()
        {
            Id = System.Guid.NewGuid().ToString();
            SettingsData = new SettingsData();
            PaddleData = new PaddleData();
            LevelData = new LevelData();
            BricksData = new BricksData();
        }
    }
}