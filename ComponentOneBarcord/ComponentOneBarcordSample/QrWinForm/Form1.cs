using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
      if (size < 0)
        size = 0;
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

    private void SaveButton_Click(object sender, EventArgs e)
    {
      // ファイル保存ダイアログを出す
      var saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "PNG Image|*.png";
      saveFileDialog.Title = "Save an Image File";
      saveFileDialog.DefaultExt = ".png";
      saveFileDialog.FileName = "BarCodeControlSample.png";
      saveFileDialog.InitialDirectory
        = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      saveFileDialog.ShowDialog();
      if (! string.IsNullOrEmpty(saveFileDialog.FileName))
      {
        // 画像をファイルに保存する
        this.BarCode1.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
      }
    }
  }
}
