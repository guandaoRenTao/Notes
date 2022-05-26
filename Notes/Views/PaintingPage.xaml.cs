using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Notes.TouchTracking;
using Notes.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace Notes.Views
{
    public partial class PaintingPage : PopupPage
    {
        public bool isUndoEnabled = false;
        public bool IsUndoEnabled
        {
            get { return isUndoEnabled; }
            set { isUndoEnabled = value; 
                OnPropertyChanged();
            }
        }
        public bool isRedoEnabled = false;
        public bool IsRedoEnabled
        {
            get{ return isRedoEnabled;}
            set { isRedoEnabled = value; OnPropertyChanged(); }
        }
        public bool isErasing = false;
        public bool IsErasing
        {
            get { return isErasing; }
            set { isErasing = value; OnPropertyChanged();}
        }
        public Color currentColor;
        public Color CurrentColor
        {
            get { return paint.Color.ToFormsColor(); }
            set
            {
                paint.Color = value.ToSKColor();
                OnPropertyChanged();
            }
        }
        public byte ColorR
        {
            get { return paint.Color.Red; }
            set
            {
                paint.Color = paint.Color.WithRed(value);
                OnPropertyChanged();
            }
        }
        public byte ColorG
        {
            get { return paint.Color.Green; }
            set
            {
                paint.Color = paint.Color.WithGreen(value);
                OnPropertyChanged();
            }
        }
        public byte ColorB
        {
            get { return paint.Color.Blue; }
            set
            {
                paint.Color = paint.Color.WithBlue(value);
                OnPropertyChanged();
            }
        }

        public float PathWidth
        {
            get { return paint.StrokeWidth; }
            set
            {
                paint.StrokeWidth = value ;
                OnPropertyChanged();
            }
        }
        public struct CompletedPath
        {
            public SKPath Path;
            public SKColor Color;
            public float Width;
        }
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<CompletedPath> completedPaths = new List<CompletedPath>();
        Stack<CompletedPath> deletedPaths = new Stack<CompletedPath>();

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        SKBitmap saveBitmap;

        public PaintingPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Create bitmap the size of the display surface
            if (saveBitmap == null)
            {
                saveBitmap = new SKBitmap(info.Width, info.Height);
            }
            // Or create new bitmap for a new size of display surface
            else if (saveBitmap.Width < info.Width || saveBitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(Math.Max(saveBitmap.Width, info.Width),
                                                  Math.Max(saveBitmap.Height, info.Height));

                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(saveBitmap, 0, 0);
                }

                saveBitmap = newBitmap;
            }

            // Render the bitmap
            canvas.Clear();
            canvas.DrawBitmap(saveBitmap, 0, 0);
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(new CompletedPath()
                        {
                            Path = inProgressPaths[args.Id],
                            Color = paint.Color,
                            Width = paint.StrokeWidth
                        });
                        inProgressPaths.Remove(args.Id);
                        deletedPaths.Clear();
                        IsRedoEnabled = false;
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;
            }
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        void UpdateBitmap()
        {
            using (SKCanvas saveBitmapCanvas = new SKCanvas(saveBitmap))
            {
                saveBitmapCanvas.Clear();
                SKColor tempColor = paint.Color;
                float tempWidth = paint.StrokeWidth;
                foreach (var path in completedPaths)
                {
                    paint.Color = path.Color;
                    paint.StrokeWidth = path.Width;
                    saveBitmapCanvas.DrawPath(path.Path, paint);
                }
                paint.Color = tempColor;
                paint.StrokeWidth = tempWidth;
                foreach (SKPath path in inProgressPaths.Values)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                }
            }

            canvasView.InvalidateSurface();
            if(completedPaths.Count > 0)
                IsUndoEnabled = true;
            else
                IsUndoEnabled = false;
        }

        private void clearBtn_Clicked(object sender, EventArgs e)
        {
            completedPaths.Clear();
            deletedPaths.Clear();
            IsRedoEnabled = false;
            inProgressPaths.Clear();
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }

        private async void okBtn_Clicked(object sender, EventArgs e)
        {
            using (SKImage image = SKImage.FromBitmap(saveBitmap))
            {
                SKData data = image.Encode();
                DateTime dt = DateTime.Now;
                string filename = String.Format("FingerPaint-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
                bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "FingerPaint", filename);

                if (!result)
                {
                    await DisplayAlert("FingerPaint", "Не получилось сохранить", "OK");
                }
                MessagingCenter.Send<PaintingPage, string>(this, "image", filename);
            }
            await PopupNavigation.Instance.PopAsync();
        }

        private void color_Clicked(object sender, EventArgs e)
        {
            colorPopup.IsOpen = true;
        }

        private void erase_Clicked(object sender, EventArgs e)
        {
            //CanvasColor = Color.White;
            IsErasing = !IsErasing;
            if (IsErasing)
            {
                erase.BackgroundColor = Color.FromHex("#a0a0a0");
                currentColor = paint.Color.ToFormsColor();
                paint.BlendMode = SKBlendMode.Src;
                paint.Color = SKColors.Transparent;
            }
            else
            {
                erase.BackgroundColor = Color.Transparent;
                //paint.BlendMode = SKBlendMode.SrcOver;
                paint.Color = currentColor.ToSKColor();
            }
        }

        private void undo_Clicked(object sender, EventArgs e)
        {
            deletedPaths.Push(completedPaths[completedPaths.Count - 1]);
            IsRedoEnabled = true;
            completedPaths.RemoveAt(completedPaths.Count - 1);
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }
        private void redo_Clicked(object sender, EventArgs e)
        {
            completedPaths.Add(deletedPaths.Pop());
            if (deletedPaths.Count == 0)
                IsRedoEnabled = false;
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }

        private void thickness_Clicked(object sender, EventArgs e)
        {
            thicknessPopup.IsOpen = true;
        }
    }
}