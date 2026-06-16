using Core.Data;
using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Entities.Select;

namespace CoreGame.Entities.Characters.Hero
{
    public class HeroSelectController : ICharacterComponent, ISubscriber
    {
        public Condition Condition { get; } = new();
        
        private readonly SelectService _selectService;
        private readonly Hero _hero;


        public HeroSelectController(Hero hero)
        {
            _hero = hero;

            _selectService = Container.Instance.GetService<SelectService>();
        }

        public void Subscribe()
        {
            _hero.SelectTrigger.Hovered += _onHovered;
        }

        public void Unsubscribe()
        {
            _hero.SelectTrigger.Hovered -= _onHovered;
        }

        private void _onHovered(SelectTrigger other, bool isHovered)
        {
            if (isHovered)
            {
                _selectService.SetSelection(other);
            }
            else
            {
                _selectService.ClearSelection();
            }
        }
    }
}