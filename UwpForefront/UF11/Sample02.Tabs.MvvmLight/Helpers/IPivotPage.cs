using System.Threading.Tasks;

namespace Sample02.Tabs.MvvmLight.Helpers
{
    public interface IPivotPage
    {
        Task OnPivotSelectedAsync();

        Task OnPivotUnselectedAsync();
    }
}
