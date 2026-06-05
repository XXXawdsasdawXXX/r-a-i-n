using Core.StateMachine;
using CoreGame.Card.Data;
using Cysharp.Threading.Tasks;

namespace CoreGame.Card.Logic.StateMachine
{
    public class EndBattleState : IBattleState
    {
        public EBattlePhase Phase => EBattlePhase.Finished;

        public bool IsInitialized { get; set; }

        public EndBattleState(BattleStateMachine machine)
        {
        }
        
        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }

    }
}