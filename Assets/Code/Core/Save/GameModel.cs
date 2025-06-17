using System;
using System.Collections.Generic;
using Core.ServiceLocator;

namespace Core.Save
{
    [Serializable]
    public class GameModel : IService
    {
        public HeroModel Hero;
        public WorldModel World;
        
        public List<HeroModel> Heroes;
        public List<WorldModel> Worlds;

        public int LastHeroIndex;
        public int LastWorldIndex;
        
        public GameModel()
        {
            World = new WorldModel();
            Hero = new HeroModel();
            
            Heroes = new List<HeroModel>();
            Worlds = new List<WorldModel>();
        }
        
        public void CopyFrom(GameModel model)
        {
            World = model?.World ?? new WorldModel();
            Hero = model?.Hero ?? new HeroModel();
            
            Heroes = model?.Heroes ?? new List<HeroModel>();
            Worlds = model?.Worlds ?? new List<WorldModel>();
        }
    }
}