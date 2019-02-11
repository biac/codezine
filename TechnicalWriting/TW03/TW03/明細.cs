namespace TW03
{
  internal class 明細
  {
    public string 商品ID { get; set; }
    public int 個数 { get; set; }
    public decimal 税別単価 { get; set; }
    public decimal 消費税率 { get; set; }
    public decimal 消費税単価 { get; set; } // = 税別単価 * 消費税率
    public decimal 税込単価 { get; set; } // = 税別単価 + 消費税単価
    public decimal 金額; // = 個数 * 税込単価
  }
}
