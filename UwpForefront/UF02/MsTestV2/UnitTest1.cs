using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using static System.Console;
using System.Linq;

namespace MsTestV2
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public async Task PicturesLibraryTest()
    {
      // My Picture フォルダーのファイル一覧を取得してみる
      var files = await Windows.Storage.KnownFolders.PicturesLibrary.GetFilesAsync();
      Assert.IsNotNull(files);

      var myPictures = string.Join("\n", files.Select(f => f.Name));
      WriteLine(myPictures);
    }

    [TestMethod]
    public async Task CodecQueryTest()
    {
      // 15063 以降
      var cq = new Windows.Media.Core.CodecQuery();
      var codecList 
        = await cq.FindAllAsync(
                  Windows.Media.Core.CodecKind.Audio,
                  Windows.Media.Core.CodecCategory.Decoder,
                  Windows.Media.Core.CodecSubtypes.AudioFormatDolbyAC3);
      var codec = codecList.FirstOrDefault();

      Assert.IsNotNull(codec);
      Assert.AreEqual(Windows.Media.Core.CodecCategory.Decoder, codec.Category);
      StringAssert.Contains(codec.DisplayName, "Dolby");

      WriteLine(codec.DisplayName);
      // 出力："Microsoft Dolby Digital Plus Decoder MFT"
    }
  }
}
