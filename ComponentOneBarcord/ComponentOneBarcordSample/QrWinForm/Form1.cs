using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QrWinForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      AdjustBarCode();
    }

    private void Form1_SizeChanged(object sender, EventArgs e)
    {
      AdjustBarCode();
    }

    void AdjustBarCode()
    {
      var size = Math.Min(this.BarCodeArea.Height, this.BarCodeArea.Width) - 20;
      this.BarCode1.Width = size;
      this.BarCode1.Height = size;
      this.BarCode1.BarHeight = size;
      this.BarCode1.Location
        = new Point( (this.BarCodeArea.Width - size) / 2, (this.BarCodeArea.Height - size) / 2);
    }

    private void InputTextBox_TextChanged(object sender, EventArgs e)
    {
      this.BarCode1.Text = this.InputTextBox.Text;
    }
  }
}
