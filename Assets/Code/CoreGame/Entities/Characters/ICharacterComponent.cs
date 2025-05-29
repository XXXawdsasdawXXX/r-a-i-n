using Core.Data;

namespace CoreGame.Entities.Characters
{
    public interface ICharacterComponent
    {
        Condition Condition { get; }
    }
}