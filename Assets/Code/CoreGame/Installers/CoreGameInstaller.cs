using System;
using Code.CoreGame._Test;
using Code.CoreGame.Camera;
using Code.CoreGame.Entities.Characters.Controllers;
using Code.CoreGame.Grid;
using Code.CoreGame.Time;
using Core.ServiceLocator;
using UnityEngine;

namespace Code.CoreGame.Installers
{
    [CreateAssetMenu(fileName = "Installer_CoreGame", menuName = "Game/Installers/CoreGame")]
    public  class CoreGameInstaller : Installer
    {
        public override Type[] GetTypes()
        {
            return new[]
            {
                typeof(TestService),
                //world
                typeof(GameTime),
                typeof(GridService),
                typeof(WorldMaterialController),
                typeof(CameraController),
                //hero
                typeof(Movement),
                typeof(Miner),
            };
        }
    }
}