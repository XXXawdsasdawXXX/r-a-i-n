using Core.Data;

namespace Code.CoreGame.Entities.Characters
{
    public interface ICharacterComponent
    {
        Condition Condition { get; }
    }
}