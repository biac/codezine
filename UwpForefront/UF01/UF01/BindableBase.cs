using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UF01
{
  /// <summary>
  /// モデルを簡略化するための <see cref="INotifyPropertyChanged"/> の実装。
  /// </summary>
  [Windows.Foundation.Metadata.WebHostHidden]
  public abstract class BindableBase : INotifyPropertyChanged
  {
    /// <summary>
    /// プロパティの変更を通知するためのマルチキャスト イベント。
    /// </summary>
    // https://stackoverflow.com/a/45422891/1327929
    private readonly List<(SynchronizationContext context, PropertyChangedEventHandler handler)> _handlers = new List<(SynchronizationContext context, PropertyChangedEventHandler handler)>();
    public event PropertyChangedEventHandler PropertyChanged
    {
      add => _handlers.Add((SynchronizationContext.Current, value));
      remove
      {
        var i = 0;
        foreach (var item in _handlers)
        {
          if (item.handler.Equals(value))
          {
            _handlers.RemoveAt(i);
            break;
          }
          i++;
        }
      }
    }


    /// <summary>
    /// プロパティが既に目的の値と一致しているかどうかを確認します。必要な場合のみ、
    /// プロパティを設定し、リスナーに通知します。
    /// </summary>
    /// <typeparam name="T">プロパティの型。</typeparam>
    /// <param name="storage">get アクセス操作子と set アクセス操作子両方を使用したプロパティへの参照。</param>
    /// <param name="value">プロパティに必要な値。</param>
    /// <param name="propertyName">リスナーに通知するために使用するプロパティの名前。
    /// この値は省略可能で、
    /// CallerMemberName をサポートするコンパイラから呼び出す場合に自動的に指定できます。</param>
    /// <returns>値が変更された場合は true、既存の値が目的の値に一致した場合は
    /// false です。</returns>
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
    {
      if (object.Equals(storage, value)) return false;

      storage = value;
      this.OnPropertyChanged(propertyName);
      return true;
    }

    /// <summary>
    /// プロパティ値が変更されたことをリスナーに通知します。
    /// </summary>
    /// <param name="propertyName">リスナーに通知するために使用するプロパティの名前。
    /// この値は省略可能で、
    /// <see cref="CallerMemberNameAttribute"/> をサポートするコンパイラから呼び出す場合に自動的に指定できます。</param>
    protected Task OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      // https://stackoverflow.com/a/45422891/1327929
      var args = new PropertyChangedEventArgs(propertyName);
      var tasks = _handlers
          .GroupBy(x => x.context, x => x.handler)
          .Select(g => invokeContext(g.Key, g));
      return Task.WhenAll(tasks);

      Task invokeContext(SynchronizationContext context, IEnumerable<PropertyChangedEventHandler> l)
      {
        if (context != null)
        {
          var tcs = new TaskCompletionSource<bool>();
          context.Post(o =>
          {
            try { invokeHandlers(l); tcs.TrySetResult(true); }
            catch (Exception e) { tcs.TrySetException(e); }
          }, null);
          return tcs.Task;
        }
        else
        {
          return Task.Run(() => invokeHandlers(l));
        }
      }
      void invokeHandlers(IEnumerable<PropertyChangedEventHandler> l)
      {
        foreach (var h in l)
          h(this, args);
      }
    }
  }
}
