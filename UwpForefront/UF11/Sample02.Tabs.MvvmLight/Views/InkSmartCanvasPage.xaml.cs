using System;

using Sample02.Tabs.MvvmLight.Helpers;
using Sample02.Tabs.MvvmLight.Services.Ink;
using Sample02.Tabs.MvvmLight.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample02.Tabs.MvvmLight.Views
{
    // For more information regarding Windows Ink documentation and samples see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/ink.md
    public sealed partial class InkSmartCanvasPage : Page
    {
        private InkSmartCanvasViewModel ViewModel
        {
            get { return ViewModelLocator.Current.InkSmartCanvasViewModel; }
        }

        public InkSmartCanvasPage()
        {
            InitializeComponent();

            Loaded += (sender, eventArgs) =>
            {
                SetCanvasSize();

                var strokeService = new InkStrokesService(inkCanvas.InkPresenter);
                var analyzer = new InkAsyncAnalyzer(inkCanvas, strokeService);
                var selectionRectangleService = new InkSelectionRectangleService(inkCanvas, selectionCanvas, strokeService);

                ViewModel.Initialize(
                    strokeService,
                    new InkLassoSelectionService(inkCanvas, selectionCanvas, strokeService, selectionRectangleService),
                    new InkNodeSelectionService(inkCanvas, selectionCanvas, analyzer, strokeService, selectionRectangleService),
                    new InkPointerDeviceService(inkCanvas),
                    new InkUndoRedoService(inkCanvas, strokeService),
                    new InkTransformService(drawingCanvas, strokeService),
                    new InkFileService(inkCanvas, strokeService));

                // In tabbedpivot projects the ballpoint pen is not selected by default, so we set it explicitly
                toolbar.ActiveTool = toolbar.GetToolButton(InkToolbarTool.BallpointPen);
                toolbar.ActiveTool.IsChecked = true;
            };
        }

        private void SetCanvasSize()
        {
            inkCanvas.Width = Math.Max(canvasScroll.ViewportWidth, 1000);
            inkCanvas.Height = Math.Max(canvasScroll.ViewportHeight, 1000);
        }
    }
}
