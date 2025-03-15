using Cysharp.Threading.Tasks;

namespace Core.GameLoop
{
    public interface IGameListener
    {
    }

    public interface ISubscriber : IGameListener
    {
        UniTask Subscribe();
        void Unsubscribe();
    }
    
    public  interface IInitializeListener : IGameListener
    {
        UniTask GameInitialize();
    }

    public interface ILoadListener : IGameListener
    {
        UniTask GameLoad();
    }

    public interface IStartListener : IGameListener
    {
        UniTask GameStart();
    }

    public interface IUpdateListener : IGameListener
    {
        void GameUpdate(float deltaTime);
    }
    
    public interface IFixedUpdateListener : IGameListener
    {
        void GameFixedUpdate(float fixedDeltaTime);
    }
    
    public interface IExitListener : IGameListener
    {
        void GameExit();
    }
}