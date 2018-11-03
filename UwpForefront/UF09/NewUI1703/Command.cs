using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace NewUI1703
{
  public class Command : ICommand
  {
    private readonly Action<object> _action;
    private readonly bool _canExecute;
#pragma warning disable CS0067
    public event EventHandler CanExecuteChanged;
#pragma warning restore

    public Command(Action<object> action, bool canExecute)
    {
      this._action = action;
      this._canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute;
    public void Execute(object parameter) => _action(parameter);
  }

  public class CommandForRichEditBox : Command
  {
    private readonly Windows.UI.Xaml.Controls.RichEditBox _richEditBox;

    public CommandForRichEditBox(Action<object> action, Windows.UI.Xaml.Controls.RichEditBox richEditBox, bool canExecute)
      : base(action, canExecute)
    {
      _richEditBox = richEditBox;
    }
  }
}
