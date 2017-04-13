using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace CalenderApp1
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class TextBoxinput : UserControl
    {
        private TransformGroup textTransformGroup;
        private MatrixTransform textMatrixTransform;
        private CompositeTransform textCompositeTransform;

        static int zindex;

        static TextBoxinput()
        {
            TextProperty = DependencyProperty.Register("Text",
                typeof(string),
                typeof(TextBoxinput),
                new PropertyMetadata("?????"));
        }

        public static DependencyProperty TextProperty { private set; get; }

        public static int ZIndex
        {
            get { return ++zindex; }
        }

        public TextBoxinput()
        {
            this.InitializeComponent();

           // this.txtbox.Opacity = 100;
            this.txtbox.Background.Opacity = 0;//テキストボックスの背景を透明にする

            txtbox.ManipulationMode = ManipulationModes.All;
            textMatrixTransform = new MatrixTransform { Matrix = Matrix.Identity };
            textCompositeTransform = new CompositeTransform();

            textTransformGroup = new TransformGroup();
            textTransformGroup.Children.Add(textMatrixTransform);
            textTransformGroup.Children.Add(textCompositeTransform);

            txtbox.RenderTransform = textTransformGroup;
        }

        public string Text
        {
            set { SetValue(TextProperty, value); }
            get { return (string)GetValue(TextProperty); }
        }
        
        // コントロールに指で触れるかマウスを移動してクリックしたとき
        void OnThumbDragStarted(object sender, DragStartedEventArgs args)
        {
            Canvas.SetZIndex(this, ZIndex);
        }


      
        // 指やマウスの動き
        void OnThumbDragDelta(object sender, DragDeltaEventArgs args)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + args.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + args.VerticalChange);
        }

        private void OnTumbTapped(object sender, TappedRoutedEventArgs e)
        {
            txtbox.IsHitTestVisible = false;// テキストボックスのマウスイベントを有効にする(テキスト入力不可状態)
        }

        private void OnTumbDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            txtbox.IsHitTestVisible = true;// テキストボックスのマウスイベントを有効にする(テキスト入力可能状態)
            txtbox.BorderThickness = new Thickness(2);//文字が入力されてないとき境界線をつける
        }

        // フォーカスを失ったとき（他のコントロールがタップされたとき）
        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
 
            txtbox.IsHitTestVisible = false;// テキストボックスのマウスイベントを有効にする(テキスト入力不可状態)
            if(txtbox.Text!="")
            {
                txtbox.BorderThickness = new Thickness(0);//文字が入力されたときは境界をなしにする
            }else
                txtbox.BorderThickness = new Thickness(2);//文字が入力されてないとき境界線をつける

        }

        private void OnThumbManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            textMatrixTransform.Matrix = textTransformGroup.Value;

           // ドラッグ(移動)
           textCompositeTransform.TranslateX = e.Delta.Translation.X;
           textCompositeTransform.TranslateY = e.Delta.Translation.Y;

           // ピンチ・ストレッチ(拡大・縮小)
           textCompositeTransform.ScaleX = e.Delta.Scale;
           textCompositeTransform.ScaleY = e.Delta.Scale;

           //回転
           textCompositeTransform.Rotation = e.Delta.Rotation;
       }
       
    }
}

