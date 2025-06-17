using System;
using System.Collections.Generic;
using System.Linq;
using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Components;
using UI.Windows.Base;
using UI.Windows.MainMenu.DeleteHero;
using UI.Windows.MainMenu.HeroSettings;
using UI.Windows.MainMenu.NewHero;

namespace UI.Windows.MainMenu.Hero
{
    public class HeroWindowController : UIWindowController<HeroWindowView>,
        IInitializeListener, ILoadListener, IStartListener, ISubscriber
    {
        public event Action HeroListChanged;
        public bool IsInitialized { get; set; }
        
        private GameModel _gameModel;


        public UniTask Initialize()
        {
            _gameModel = Container.Instance.GetService<GameModel>();
            
            view.HeroesRadioGroup.Initialize();
            
            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            view.ButtonNew.Clicked += _openNewHeroWindow;
            view.ButtonDelete.Clicked += _openDeleteHeroWindow;
            view.ButtonSettings.Clicked += _openHeroSettingsWindow;
            view.HeroesRadioGroup.Selected += _updateSelectedHeroView;
     
            windowManager.GetWindow<NewHeroWindowController>().HeroCreated += _updateHeroesList;
            windowManager.GetWindow<DeleteHeroWindowController>().HeroDeleted += _updateHeroesList;
        }

        public UniTask GameLoad(GameModel model)
        {
            _updateHeroesList();

            _updateOptionButtonsView();

            _updateSelectedHeroView(model.LastHeroIndex);

            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            view.ButtonNew.Clicked -= _openNewHeroWindow;
            view.ButtonDelete.Clicked -= _openDeleteHeroWindow;
            view.ButtonSettings.Clicked -= _openHeroSettingsWindow;
            view.HeroesRadioGroup.Selected -= _updateSelectedHeroView;
         
            windowManager.GetWindow<NewHeroWindowController>().HeroCreated -= _updateHeroesList;
            windowManager.GetWindow<DeleteHeroWindowController>().HeroDeleted -= _updateHeroesList;
        }

        private void _openNewHeroWindow()
        {
            windowManager.OpenWindow<NewHeroWindowController>();
        }

        private void _openHeroSettingsWindow()
        {
            windowManager.OpenWindow<HeroSettingsWindowController>();
        }

        private void _openDeleteHeroWindow()
        {
            windowManager.OpenWindow<DeleteHeroWindowController>();
        }

        private void _updateSelectedHeroView(int heroIndex)
        {
            if (_gameModel.Heroes.Count > heroIndex && _gameModel.LastHeroIndex != heroIndex)
            {
                _gameModel.LastHeroIndex = heroIndex;
                view.BodyHeroView.SetActive(true);
                view.HeroCard.SetModel(new UIHeroCardView.Model(
                    _gameModel.Heroes[heroIndex].Name, 
                    _gameModel.Heroes[heroIndex].GameTime.ToString()));
            }
            else
            {
                view.BodyHeroView.SetActive(false);
            }
        }

        private void _updateHeroesList()
        {
            view.HeroesRadioGroup.Pool.DisableAll();

            foreach (HeroModel heroModel in _gameModel.Heroes)
            {
                UIText text = view.HeroesRadioGroup.Pool.GetNext();
                text.SetText(heroModel.Name);
            }
        }

        private void _updateOptionButtonsView()
        {
            bool isHasHero = _getLastHeroIndex(_gameModel.Heroes) >= 0;
            view.ButtonDelete.SetInteractable(isHasHero);
            view.ButtonSettings.SetInteractable(isHasHero);
        }

        private static int _getLastHeroIndex(List<HeroModel> heroes)
        {
            if (heroes == null || heroes.Count == 0)
            {
                return -1;
            }

            return heroes
                .Select((hero, index) => new { Hero = hero, Index = index })
                .OrderByDescending(x => x.Hero.LastGameExitTime)
                .First()
                .Index;
        }
    }
}