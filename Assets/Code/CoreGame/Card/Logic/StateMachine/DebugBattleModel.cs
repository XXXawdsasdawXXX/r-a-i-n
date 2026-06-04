using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Card.Data;
using Cysharp.Threading.Tasks;
using Essential;
using UnityEngine;

namespace CoreGame.Card.Logic.StateMachine
{
    public class DebugBattleModel : Essential.Mono, IService, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }

        [SerializeField] private BattleModel _model;
        private BattleService _battleService;

        public UniTask Initialize()
        {
            Log.Info(this, "Initialize", Color.black);
            _battleService = Container.Instance.GetService<BattleService>();
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            Log.Info(this, "Subscribe", Color.black);

            _battleService.BattleStarted  += _updateModel;
            _battleService.TurnStarted    += _updateModel;
            _battleService.CardPlayed     += _updateModel;
            _battleService.BattleFinished += _updateModel;
        }

        public void Unsubscribe()
        {
            _battleService.BattleStarted  -= _updateModel;
            _battleService.TurnStarted    -= _updateModel;
            _battleService.CardPlayed     -= _updateModel;
            _battleService.BattleFinished -= _updateModel;
        }

        private void _updateModel(BattleModel model)
        {
            Log.Info(this, "Update model", Color.black);

            _model = model;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}