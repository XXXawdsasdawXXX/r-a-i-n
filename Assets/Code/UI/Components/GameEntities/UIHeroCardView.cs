using UnityEngine;

namespace UI.Components
{
    public class UIHeroCardView : Essential.Mono
    {
        public struct Model
        {
            public readonly string Name;
            public readonly string GameTime;

            public Model(string name, string gameTime)
            {
                Name = name;
                GameTime = gameTime;
            }
        }

        [SerializeField] private UIText _textHeroName;
        [SerializeField] private UIText _textGameTime;
        
        public void SetModel(Model model)
        {
            _textHeroName.SetText(model.Name);
            _textGameTime.SetText(model.GameTime);
        }
    }
}