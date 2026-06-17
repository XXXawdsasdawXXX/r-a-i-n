using System.Collections.Generic;
using System.Linq;
using Core.Audio;
using Core.Input;
using Core.Localization;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;

namespace UI.Windows.Game.Pause
{
    public class PauseWindowController : UIWindowController<PauseWindowView>
    {
        private AudioGlobalVolume _audioGlobalVolume;
        private InputService _inputService;
        private LocalizationService _localization;

        private bool _isOpened;

        public override UniTask InitializeWindow(UIWindowManager manager)
        {
            _audioGlobalVolume = Container.Instance.GetService<AudioGlobalVolume>();
            _inputService = Container.Instance.GetService<InputService>();
            _localization = Container.Instance.GetService<LocalizationService>();

            return base.InitializeWindow(manager);
        }

        public override void StartWindow()
        {
            _refreshLanguageDropdown();
        }

        public override void LoadWindow(GameModel model)
        {
            view.SliderMusic.SetValueWithoutNotify(_audioGlobalVolume.MusicValue);
            view.SliderSFX.SetValueWithoutNotify(_audioGlobalVolume.SFXValue);
            _refreshLanguageDropdown();

            base.LoadWindow(model);
        }

        public override void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _inputService.ActionEnded += InputServiceOnActionEnded;
                view.SliderMusic.Changed += _audioGlobalVolume.SetMusicVolume;
                view.SliderSFX.Changed += _audioGlobalVolume.SetSFXVolume;

                if (view.LanguageDropDown != null)
                {
                    view.LanguageDropDown.Changed += _onLanguageChanged;
                }

                if (_localization != null)
                {
                    _localization.LocaleChanged += _onLocaleChanged;
                }
            }
            else
            {
                _inputService.ActionEnded -= InputServiceOnActionEnded;
                view.SliderMusic.Changed -= _audioGlobalVolume.SetMusicVolume;
                view.SliderSFX.Changed -= _audioGlobalVolume.SetSFXVolume;

                if (view.LanguageDropDown != null)
                {
                    view.LanguageDropDown.Changed -= _onLanguageChanged;
                }

                if (_localization != null)
                {
                    _localization.LocaleChanged -= _onLocaleChanged;
                }
            }
        }

        private void InputServiceOnActionEnded(EInputAction action)
        {
            if (action is EInputAction.Esc)
            {
                if (_isOpened)
                {
                    Close();
                }
                else
                {
                    Open();
                }

                _isOpened = !_isOpened;
            }
        }

        private void _onLanguageChanged(int index)
        {
            _localization?.SetLocaleByIndex(index);
        }

        private void _onLocaleChanged()
        {
            _refreshLanguageDropdown();
        }

        private void _refreshLanguageDropdown()
        {
            if (view.LanguageDropDown == null || _localization == null || !_localization.IsInitialized)
            {
                return;
            }

            IReadOnlyList<LocaleOption> options = _localization.GetLocaleOptions();
            if (options.Count == 0)
            {
                return;
            }

            view.LanguageDropDown.SetOptions(options.Select(option => option.DisplayName).ToList());
            view.LanguageDropDown.SetCurrentWithoutNotify(_localization.GetSelectedLocaleIndex());
        }
    }
}
