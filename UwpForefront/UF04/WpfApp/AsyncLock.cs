using System;
using System.Threading.Tasks;


// .NET TIPS：非同期：awaitを含むコードをロックするには？（AsyncLock編）［C#、VB］
// http://www.atmarkit.co.jp/ait/articles/1411/18/news135.html

namespace WpfApp
{
  public sealed class AsyncLock
  {
    private readonly System.Threading.SemaphoreSlim m_semaphore
      = new System.Threading.SemaphoreSlim(1, 1);
    private readonly Task<IDisposable> m_releaser;

    public AsyncLock()
    {
      m_releaser = Task.FromResult((IDisposable)new Releaser(this));
    }

    public Task<IDisposable> LockAsync()
    {
      var wait = m_semaphore.WaitAsync();
      return wait.IsCompleted ?
              m_releaser :
              wait.ContinueWith(
                (_, state) => (IDisposable)state,
                m_releaser.Result,
                System.Threading.CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
              );
    }
    private sealed class Releaser : IDisposable
    {
      private readonly AsyncLock m_toRelease;
      internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
      public void Dispose() { m_toRelease.m_semaphore.Release(); }
    }
  }
}
