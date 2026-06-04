using System.Linq;
using Core.GameLoop;
using Core.Input;
using Core.Save;
using Core.ServiceLocator;
using CoreGame.Card;
using CoreGame.Entities.Animation;
using CoreGame.Entities.Characters.Controllers;
using CoreGame.Entities.Params;
using Essential;
using UnityEngine;

namespace CoreGame.Entities.Characters.Hero
{
    public class Hero : Character, ISubscriber
    {
        public HeroModel Model { get; private set; }
        
        [field: Header("Unity components")]
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        
        [field: Space]
        
        [field: Header("Net components")]
        [field: SerializeField] public PersonName Name { get; private set; }
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public HeroColor Color { get; private set; }
        [field: SerializeField] public HeroAnimation Animation { get; private set; }
        [field: SerializeField] public HeroItemController ItemController { get; private set; }
        
        
        public override void InitializeComponents()
        {
            Log.Info(this, $"Initialize hero.\nis owner = {IsOwner}.\n owner id = {OwnerId}", UnityEngine.Color.black);
     
            if (IsOwner)
            {
                InputManager input = Container.Instance.GetService<InputManager>();
                HeroSettings heroSettings = Container.Instance.GetSO<HeroSettings>();
                CardLibrary cardLibrary = Container.Instance.GetSO<CardLibrary>();
                GameModel gameModel = Container.Instance.GetService<GameModel>();
                
                Model = _getHeroModel(gameModel.Hero, cardLibrary); 
                
                Movement movement = new(Rigidbody, input.Direction, heroSettings.MoveSpeed);
                Components.Add(typeof(Movement), movement);
                
                Mainer mainer = new(Animation, Health);
                Components.Add(typeof(Mainer), mainer);
                
                movement.Condition.Add(() => Health.Current > 0);
                movement.Condition.Add(() => Animation.CurrentState is not 
                    AnimatorKey.ECharacterAnimationState.EAT and not 
                    AnimatorKey.ECharacterAnimationState.HARVEST);
                
                mainer.Condition.Add(() => input.Direction.Value == Vector2.zero);
                mainer.Condition.Add(() => Health.Current > 0);
                
                Health.Set(Model.Health);
                Name.SetName(Model.Name);
            }

            IsConstructed = true;
        }

        public void Subscribe()
        {
            Health.Changed += _onHealthChanged;
        }

        public void Unsubscribe()
        {   
            Health.Changed -= _onHealthChanged;
        }

        private HeroModel _getHeroModel(HeroModel hero, CardLibrary library)
        {
            hero.HeroId = OwnerId.ToString();
         
            if (hero.Deck == null || hero.Deck.Count == 0)
            {
                hero.Deck = library.DefaultCardsDeck.ToList();
            }

            return hero;
        }

        private void _onHealthChanged()
        {
            if (IsOwner && Model != null)
            {
                Model.Health = Health.Current;
            }
        }
    }
}