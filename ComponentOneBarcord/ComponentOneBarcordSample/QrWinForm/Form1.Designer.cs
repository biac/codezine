namespace QrWinForm
{
  partial class Form1
  {
    /// <summary>
    /// 必要なデザイナー変数です。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 使用中のリソースをすべてクリーンアップします。
    /// </summary>
    /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows フォーム デザイナーで生成されたコード

    /// <summary>
    /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
    /// コード エディターで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      this.panel1 = new System.Windows.Forms.Panel();
      this.InputTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.panel2 = new System.Windows.Forms.Panel();
      this.SaveButton = new System.Windows.Forms.Button();
      this.BarCodeArea = new System.Windows.Forms.Panel();
      this.BarCode1 = new C1.Win.BarCode.C1BarCode();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.BarCodeArea.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.AutoSize = true;
      this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add(this.InputTextBox);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 25);
      this.panel1.Size = new System.Drawing.Size(484, 59);
      this.panel1.TabIndex = 0;
      // 
      // InputTextBox
      // 
      this.InputTextBox.Location = new System.Drawing.Point(102, 12);
      this.InputTextBox.Name = "InputTextBox";
      this.InputTextBox.Size = new System.Drawing.Size(189, 19);
      this.InputTextBox.TabIndex = 1;
      this.InputTextBox.Text = "WindowsアプリでバーコードやQRコードを簡単出力！";
      this.InputTextBox.TextChanged += new System.EventHandler(this.InputTextBox_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 12);
      this.label1.TabIndex = 0;
      this.label1.Text = "表示する文字列";
      // 
      // panel2
      // 
      this.panel2.AutoSize = true;
      this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel2.Controls.Add(this.SaveButton);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel2.Location = new System.Drawing.Point(0, 284);
      this.panel2.Name = "panel2";
      this.panel2.Padding = new System.Windows.Forms.Padding(0, 25, 0, 0);
      this.panel2.Size = new System.Drawing.Size(484, 52);
      this.panel2.TabIndex = 1;
      // 
      // SaveButton
      // 
      this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.SaveButton.Location = new System.Drawing.Point(359, 17);
      this.SaveButton.Name = "SaveButton";
      this.SaveButton.Size = new System.Drawing.Size(100, 23);
      this.SaveButton.TabIndex = 0;
      this.SaveButton.Text = "画像を保存";
      this.SaveButton.UseVisualStyleBackColor = true;
      // 
      // BarCodeArea
      // 
      this.BarCodeArea.BackColor = System.Drawing.Color.White;
      this.BarCodeArea.Controls.Add(this.BarCode1);
      this.BarCodeArea.Dock = System.Windows.Forms.DockStyle.Fill;
      this.BarCodeArea.Location = new System.Drawing.Point(0, 59);
      this.BarCodeArea.Name = "BarCodeArea";
      this.BarCodeArea.Size = new System.Drawing.Size(484, 225);
      this.BarCodeArea.TabIndex = 2;
      // 
      // BarCode1
      // 
      this.BarCode1.AdditionalNumber = null;
      this.BarCode1.AutoSize = false;
      this.BarCode1.BackColor = System.Drawing.Color.White;
      this.BarCode1.BarHeight = 250;
      this.BarCode1.CodeType = C1.BarCode.CodeType.QRCode;
      this.BarCode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
      this.BarCode1.ForeColor = System.Drawing.Color.Black;
      this.BarCode1.Location = new System.Drawing.Point(14, 16);
      this.BarCode1.Margin = new System.Windows.Forms.Padding(0);
      this.BarCode1.Name = "BarCode1";
      this.BarCode1.QRCodeOptions.ErrorLevel = C1.BarCode.QRCodeErrorLevel.High;
      this.BarCode1.QuietZone.Bottom = 0D;
      this.BarCode1.QuietZone.Left = 0D;
      this.BarCode1.QuietZone.Right = 0D;
      this.BarCode1.QuietZone.Top = 0D;
      this.BarCode1.Size = new System.Drawing.Size(250, 250);
      this.BarCode1.TabIndex = 0;
      this.BarCode1.Text = "WindowsアプリでバーコードやQRコードを簡単出力！";
      this.BarCode1.TextFixedLength = 0;
      this.BarCode1.WideToNarrowRatio = 2F;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(484, 336);
      this.Controls.Add(this.BarCodeArea);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.BarCodeArea.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox InputTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button SaveButton;
    private System.Windows.Forms.Panel BarCodeArea;
    private C1.Win.BarCode.C1BarCode BarCode1;
  }
}

