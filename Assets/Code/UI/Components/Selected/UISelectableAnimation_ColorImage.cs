using Core.GameLoop;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Data;
using UnityEngine;

namespace UI.Components
{
    public class UISelectableAnimation_ColorImage : UISelectableAnimation, IInitializeListener
    {
        public bool IsInitialized { get; set; }

        [SerializeField] private UIImage _uiImage;

        private UISettings _uiSettings;

        public UniTask Initialize()
        {
            _uiSettings = Container.Instance.GetConfig<UISettings>();
      
            return UniTask.CompletedTask;
        }

        public override void Select()
        {
            _uiImage.Colorize(_uiSettings.SelectedTween);
        }

        public override void Deselect()
        {
            _uiImage.Colorize(_uiSettings.DeselectedTween);
        }
    }
}