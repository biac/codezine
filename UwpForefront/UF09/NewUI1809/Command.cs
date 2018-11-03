using System;
using System.Windows.Input;

namespace NewUI1809
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

    public CommandForRichEditBox(Action<object> action,
      Windows.UI.Xaml.Controls.RichEditBox richEditBox, bool canExecute)
      : base(action, canExecute)
    {
      _richEditBox = richEditBox;
    }
  }
}
