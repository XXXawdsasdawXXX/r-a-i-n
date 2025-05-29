using FishNet.Broadcast;

namespace CoreGame.InteractionObjects.Activators
{
    public struct ActivatorBroadcast : IBroadcast
    {
        public string ObjectID;
        public bool IsActive;
    }
}