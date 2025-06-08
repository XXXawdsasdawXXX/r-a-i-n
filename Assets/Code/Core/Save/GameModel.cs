using System;
using Core.ServiceLocator;

namespace Core.Save
{
    [Serializable]
    public class GameModel : IService
    {
        public HeroModel Hero;
        public WorldModel World;
        
        public GameModel()
        {
            World = new WorldModel();
            Hero = new HeroModel();
        }
        
        public void CopyFrom(GameModel model)
        {
            World = model?.World ?? new WorldModel();
            Hero = model?.Hero ?? new HeroModel();
        }
    }
}