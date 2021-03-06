using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Lagou.Controls;
using Lagou.Droid.Renders;
using Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(Border), typeof(BorderRender))]
namespace Lagou.Droid.Renders {
    public class BorderRender : VisualElementRenderer<Border> {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);
            BorderRendererVisual.UpdateBackground(Element, this.ViewGroup);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Border> e) {
            base.OnElementChanged(e);
            BorderRendererVisual.UpdateBackground(Element, this.ViewGroup);
        }

        protected override void DispatchDraw(Canvas canvas) {
            if (Element.IsClippedToBorder) {
                canvas.Save(SaveFlags.Clip);
                BorderRendererVisual.SetClipPath(this, canvas);
                base.DispatchDraw(canvas);
                canvas.Restore();
            } else {
                base.DispatchDraw(canvas);
            }
        }
    }


    public static class BorderRendererVisual {
        public static void UpdateBackground(Border border, Android.Views.View view) {
            var strokeThickness = border.StrokeThickness;
            var context = view.Context;

            var corners = new float[] {
                    (float)border.CornerRadius.TopLeft,
                    (float)border.CornerRadius.TopLeft,

                    (float)border.CornerRadius.TopRight,
                    (float)border.CornerRadius.TopRight,

                    (float)border.CornerRadius.BottomRight,
                    (float)border.CornerRadius.BottomRight,

                    (float)border.CornerRadius.BottomLeft,
                    (float)border.CornerRadius.BottomLeft
                };

            GradientDrawable dab = null;
            dab = new GradientDrawable();

            if (strokeThickness.HorizontalThickness + strokeThickness.VerticalThickness > 0) {
                dab.SetColor(border.BackgroundColor.ToAndroid());
                dab.SetStroke((int)context.ToPixels(strokeThickness.Max()), border.Stroke.ToAndroid());
            }

            dab.SetCornerRadii(corners);
            dab.SetColor(border.BackgroundColor.ToAndroid());
            dab.SetCornerRadii(corners);
            view.Background = dab;

            view.SetPadding(
                (int)context.ToPixels(strokeThickness.Left + border.Padding.Left),
                (int)context.ToPixels(strokeThickness.Top + border.Padding.Top),
                (int)context.ToPixels(strokeThickness.Right + border.Padding.Right),
                (int)context.ToPixels(strokeThickness.Bottom + border.Padding.Bottom));
        }

        static double Max(this Thickness t) {
            return new double[] {
                t.Left,
                t.Top,
                t.Right,
                t.Bottom
            }.Max();
        }

        static double Max(this CornerRadius t) {
            return new double[] { t.TopLeft, t.TopRight, t.BottomRight, t.BottomLeft }.Max();
        }

        public static void SetClipPath(this BorderRender br, Canvas canvas) {
            var clipPath = new Path();
            var corner = br.Element.CornerRadius;
            var tl = (float)corner.TopLeft;
            var tr = (float)corner.TopRight;
            var bbr = (float)corner.BottomRight;
            var bl = (float)corner.BottomLeft;

            //Array of 8 values, 4 pairs of [X,Y] radii
            float[] radius = new float[] {
                tl, tl, tr, tr, bbr, bbr, bl, bl
            };

            int w = (int)br.Width;
            int h = (int)br.Height;

            clipPath.AddRoundRect(new RectF(
                br.ViewGroup.PaddingLeft,
                br.ViewGroup.PaddingTop,
                w - br.ViewGroup.PaddingRight,
                h - br.ViewGroup.PaddingBottom),
                radius,
                Path.Direction.Cw);

            canvas.ClipPath(clipPath);
        }
    }
}