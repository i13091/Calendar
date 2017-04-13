using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace CalenderApp1
{
    public sealed partial class ThicknessSettingDialog : UserControl
    {

        AppSettings appSettings;

        // 引数にAppSettingsのオブジェクトを取るように変更(2016/09/06)
        public ThicknessSettingDialog(AppSettings appSettings)
        {
            this.appSettings = appSettings;

            this.InitializeComponent();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        // カラーピッカーで変更された色をデフォルトの値として登録
        private void picker_ColorChanged(object sender, Windows.UI.Color color)
        {
            this.appSettings.SelectedColor = color;
        }

    }
}
