using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Core.Ui.Models;
using MicaForEveryone.Core.Ui.ViewModels;
using XclParser.Reflection;

<<<<<<< HEAD:MicaForEveryone.Core/Models/ProcessRule.cs
namespace MicaForEveryone.Core.Models
=======
using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Models
>>>>>>> 87676fa (feat: change titlebar color):MicaForEveryone/Models/ProcessRule.cs
{
    [XclType(TypeName = "Process")]
    public class ProcessRule : IRule
    {
        [XclConstructor]
        public ProcessRule(string processName)
        {
            ProcessName = processName;
        }

        public string Name => $"Process({ProcessName})";

        public int Priority => 1;

        [XclParameter]
        public string ProcessName { get; }

        [XclField]
        public TitlebarColorMode TitleBarColor { get; set; }

        [XclField]
        public BackdropType BackdropPreference { get; set; }

        [XclField]
        public CornerPreference CornerPreference { get; set; }

        [XclField]
        public bool ExtendFrameIntoClientArea { get; set; }

        [XclField]
        public bool EnableBlurBehind { get; set; }

        [XclField]
        public string CaptionColor { get; set; } = string.Empty;

        [XclField]
        public string CaptionTextColor { get; set; } = string.Empty;

        [XclField]
        public string BorderColor { get; set; } = string.Empty;

        public bool IsApplicable(TargetWindow target) =>
            target.ProcessName == ProcessName;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(ISettingsViewModel parent, IRuleSettingsViewModel viewModel)
        {
            viewModel.ParentViewModel = parent;
            viewModel.Rule = this;
            return new RulePaneItem(ProcessName, PaneItemType.Process, viewModel);
        }
    }
}
