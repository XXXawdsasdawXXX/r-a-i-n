using System;
using System.Collections.Generic;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Core.StateMachine
{
    public abstract class StateMachine<TState> : IService where TState : IState
    {
        protected Dictionary<Type, TState> states;
        
        protected TState currentState;
        
        public async void SwitchState(Type type)
        {
            try
            {
                if (currentState != null)
                {
                    await currentState.Exit();
                }

                await setState(type);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected virtual async UniTask setState(Type type)
        {
            try
            {
                currentState = states[type];

                if (!currentState.IsInitialized)
                {
                    await currentState.Initialize();

                    currentState.IsInitialized = true;
                }
            
                await states[type].Enter();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}