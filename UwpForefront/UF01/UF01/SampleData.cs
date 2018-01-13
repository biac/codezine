using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace UF01
{
  public class SampleData : BindableBase
  {
    private Color _sampleColor = ColorHelper.FromArgb(0xFF, 0x0, 0xA2, 0xE8);
    public Color SampleColor
    {
      get => _sampleColor;
      set => SetProperty(ref _sampleColor, value);
    }
  }
}
