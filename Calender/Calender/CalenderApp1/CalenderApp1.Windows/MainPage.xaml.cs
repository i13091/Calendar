using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.Graphics.Printing;//IPrintDocumentSourceで必要
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Printing;//PrintDocumentで必要


// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace CalenderApp1
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AppSettings appSettings = new AppSettings();
        Random rand = new Random();

        // 印刷用
        PrintDocument printDocument;
        IPrintDocumentSource printDocumentSource;

        public MainPage()
        {
            this.InitializeComponent();

            // PrintDocumentを作成し，イベントハンドラーを追加
            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            printDocument.Paginate += OnPrintDocumentPaginate;
            printDocument.GetPreviewPage += OnPrintDocumentGetPreviewPage;
            printDocument.AddPages += OnPrintDocumentAddPages;
        }

        private void PaintBtn(object sender, RoutedEventArgs e)
        {
            this.appSettings.IsPaintBtn = true;//描画モードにする
            //DisplayDialog(sender, new ThicknessSettingDialog());
            DisplayDialog(sender, new ThicknessSettingDialog(appSettings));

        }

        private void TextBtn(object sender, RoutedEventArgs e)
        {
            this.appSettings.IsPaintBtn = false;//描画モードを解除

            //DisplayDialog(sender, new TextBoxinput());
            TextBoxinput newTxtBox = new TextBoxinput
            {
            };
            // Canvas.SetLeft(newTxtBox, 400);
            // Canvas.SetTop(newTxtBox, 400);

            Canvas.SetLeft(newTxtBox, this.ActualWidth / 2 - 144 * rand.NextDouble());
            Canvas.SetTop(newTxtBox, this.ActualHeight / 2 - 144 * rand.NextDouble());
            paintCanvas.Children.Add(newTxtBox);
        }

        private void StampBtn(object sender, RoutedEventArgs e)
        {

        }

        void DisplayDialog(object sender, FrameworkElement dialog)
        {
            dialog.DataContext = appSettings;

            // DailogをPopupクラスより、Flyoutクラスへ書き換え
            Flyout flyout = new Flyout()
            {
                Content = dialog,
                Placement = FlyoutPlacementMode.Bottom
            };

            flyout.Closed += (dislogSender, dislogArgs) =>
            {
                // this.BottomAppBar.IsOpen = false;
            };
            flyout.ShowAt((FrameworkElement)sender);
        }

        /***
         * プログラミングWindows 第6版下,pp.20-23 FingerPaint3を参照
         * 
         * イベントの種類に関しては次のURLを参照:http://dev.classmethod.jp/ria/win-store-app-touch/
         */
        Dictionary<uint, Polyline> pointerDictionary = new Dictionary<uint, Polyline>();

        protected override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            if (this.appSettings.IsPaintBtn == false) return;// 描画モードでないときは下の処理をしない

            // イベント引数から情報を取得
            uint id = args.Pointer.PointerId;
            Point point = args.GetCurrentPoint(this).Position;

            // ランダムな色を作成 <<< 変更 >>>カラーは「ペイントで指定した値：appSettings.SelectedColorとする」
            // rand.NextBytes(rgb);
            //Color color = Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
            Color color = appSettings.SelectedColor;

            // Polyline を作成
            Polyline polyline = new Polyline
            {
                Stroke = new SolidColorBrush(color),
                //<<< 変更 >>>太さは「ペイントで指定した値：appSettings.Thicknessとする」
                StrokeThickness = appSettings.Thickness, //StrokeThickness = 24

                //<<<追加>>> 線の両端を丸くする
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            polyline.PointerPressed += OnPolylinePointerPressed;
            polyline.RightTapped += OnPolylineRightTapped;
            polyline.Holding += OnPolylineHolding;//<<<変更>>> RightTappedからHoldingに変更
            polyline.Points.Add(point);

            // Gridに追加
            contentGrid.Children.Add(polyline);

            // ディクショナリに追加
            pointerDictionary.Add(id, polyline);

            // ポインターをキャプチャ
            CapturePointer(args.Pointer);

            // 入力フォーカスを設定
            Focus(FocusState.Programmatic);

            base.OnPointerPressed(args);
        }

        void OnPolylinePointerPressed(object sender, PointerRoutedEventArgs args)
        {
            args.Handled = true;
        }

        void OnPolylineRightTapped(object sender, RightTappedRoutedEventArgs args)
        {
            Polyline polyline = sender as Polyline;
            //MenuFlyout popupMenu = CreateMenuFlyout(polyline);  // コードで作成
            //MenuFlyout popupMenu = GetMenuFlyout(polyline);   // リソースで定義

            // FlyoutBase.ShowAttachedFlyoutメソッドか、ShowAtメソッドを使用する
            //FlyoutBase.SetAttachedFlyout(polyline, popupMenu);
            //Flyout.ShowAttachedFlyout(polyline);
            //popupMenu.ShowAt(polyline);
        }

        void OnPolylineHolding(object sender, HoldingRoutedEventArgs args)
        {
            Polyline polyline = sender as Polyline;
            //MenuFlyout popupMenu = CreateMenuFlyout(polyline);  // コードで作成
            //popupMenu.ShowAt(polyline);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs args)
        {
            // イベント引数から情報を取得
            uint id = args.Pointer.PointerId;
            Point point = args.GetCurrentPoint(this).Position;

            // IDがディクショナリに存在する場合は、Polylineに座標を追加
            if (pointerDictionary.ContainsKey(id))
                pointerDictionary[id].Points.Add(point);

            base.OnPointerMoved(args);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            // イベント引数から情報を取得
            uint id = args.Pointer.PointerId;

            // IDがディクショナリに存在する場合は、それを削除
            if (pointerDictionary.ContainsKey(id))
                pointerDictionary.Remove(id);

            base.OnPointerReleased(args);
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs args)
        {
            // イベント引数から情報を取得
            uint id = args.Pointer.PointerId;

            // IDがディクショナリに存在する場合は描画を中止
            if (pointerDictionary.ContainsKey(id))
            {
                contentGrid.Children.Remove(pointerDictionary[id]);
                pointerDictionary.Remove(id);
            }

            base.OnPointerCaptureLost(args);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            if (args.Key == VirtualKey.Escape)
                ReleasePointerCaptures();

            base.OnKeyDown(args);
        }

        // 印刷用のメソッド
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            // Attach PrintManager handler
            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested += OnPrintManagerPrintTaskRequested;

            base.OnNavigatedTo(args);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Detach PrintManager handler
            PrintManager.GetForCurrentView().PrintTaskRequested -= OnPrintManagerPrintTaskRequested;

            base.OnNavigatedFrom(e);
        }

        void OnPrintManagerPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            args.Request.CreatePrintTask("Hello Printer", OnPrintTaskSourceRequested);
        }

        void OnPrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(printDocumentSource);
        }

        void OnPrintDocumentPaginate(object sender, PaginateEventArgs args)
        {
            printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        void OnPrintDocumentGetPreviewPage(object sender, GetPreviewPageEventArgs args)
        {
            printDocument.SetPreviewPage(args.PageNumber, contentGrid);
        }

        void OnPrintDocumentAddPages(object sender, AddPagesEventArgs args)
        {
            printDocument.AddPage(contentGrid);
            printDocument.AddPagesComplete();
        }

    }


/*
        #region コードでMenuFlyoutを作成
        private MenuFlyout CreateMenuFlyout(Polyline polyline)
        {
            var menu = new MenuFlyout() { Placement = FlyoutPlacementMode.Top };
            // 削除コマンドの設定
            var deleteCommand = new RelayCommand(arg =>
            {
                Polyline line = arg as Polyline;
                contentGrid.Children.Remove(line);
            });
            var delete = new MenuFlyoutItem()
            {
                Text = "Delete",
                Command = deleteCommand,
                CommandParameter = polyline
            };
            menu.Items.Add(delete);

            return menu;
        }
        #endregion

        #region Pageのリソース定義を利用する場合
        private MenuFlyout GetMenuFlyout(Polyline polyline)
        {
            var menu = this.Resources["popupMenu"] as MenuFlyout;
            // 削除コマンドの設定
            var deleteCommand = new RelayCommand(arg =>
            {
                Polyline line = arg as Polyline;
                contentGrid.Children.Remove(line);
            });
            var delete = GetMenuItem(menu, "menuDelete");
            delete.Command = deleteCommand;
            delete.CommandParameter = polyline;

            return menu;
        }
        private MenuFlyoutItem GetMenuItem(MenuFlyout menu, string name)
        {
            return (MenuFlyoutItem)menu.Items.Where(x => x.Name == name).FirstOrDefault();
        }
        #endregion
    }


    #region コマンドの定義
    class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        internal RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        internal RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
    #endregion
    */
}
