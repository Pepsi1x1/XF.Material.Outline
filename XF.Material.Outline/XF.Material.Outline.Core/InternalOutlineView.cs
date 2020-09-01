using System;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace XF.Material.Outline.Core
{
	[Preserve (AllMembers = true)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public class InternalOutlineView : SKCanvasView
	{
		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(InternalOutlineView), null, BindingMode.OneWay, null, LabelTextPropertyChanged);

		public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(nameof(HelperText), typeof(string), typeof(InternalOutlineView), null, BindingMode.OneWay);

		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(InternalOutlineView), null, BindingMode.OneWay, null, TintColorPropertyChanged, null, null, defaultValueCreator: TintColorDefaultValueCreator);

		public static readonly BindableProperty ForegroundColorProperty =
			BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(InternalOutlineView), null, BindingMode.OneWay, null, ForegroundColorPropertyChanged, null, null, defaultValueCreator: ForegroundColorDefaultValueCreator);

		public static readonly BindableProperty ErrorColorProperty =
			BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(InternalOutlineView), null, BindingMode.OneWay, null, ErrorColorPropertyChanged, null, null, defaultValueCreator: ErrorColorDefaultValueCreator);


		private float _currentTextSize;

		private float _currentTextY;

		internal SKColor SkTintColor = Color.FromHex("#6200ee").ToSKColor();

		internal SKColor SkForegroundColor = Color.FromHex("#FF808080").ToSKColor();

		internal SKColor SkErrorColor = Color.FromHex("#b00020").ToSKColor();

		public InternalOutlineView()
		{
			this.InputTransparent = true;

			float scale = (float) Device.Info.ScalingFactor;

			this.CornerRadius *= scale;

			this.LabelMargin *= scale;

			this.PlaceHolderFontSize *= scale;

			this.LabelFontSize *= scale;

			this.LabelPadding *= scale;
		}

		public float LabelPadding { get; } = 4f;

		public int TotalLoops { get; set; } = 50;

		public int CurrentLoopIteration { get; set; }
		
		public Color ErrorColor
		{
			get => (Color) this.GetValue(ErrorColorProperty);
			set => this.SetValue(ErrorColorProperty, value);
		}

		public Color ForegroundColor
		{
			get => (Color) this.GetValue(ForegroundColorProperty);
			set => this.SetValue(ForegroundColorProperty, value);
		}

		public Color TintColor
		{
			get => (Color) this.GetValue(TintColorProperty);
			set => this.SetValue(TintColorProperty, value);
		}

		public float CornerRadius { get; set; } = 4f;

		public float LabelMargin { get; set; } = 16f;

		public float PlaceHolderFontSize { get; set; } = 14.0f;

		public float LabelFontSize { get; set; } = 12.0f;

		public bool HasText { get; set; }

		public bool InternalIsFocused { get; set; }

		public string Placeholder
		{
			get => (string) this.GetValue(PlaceholderProperty);
			set => this.SetValue(PlaceholderProperty, value);
		}

		public string HelperText
		{
			get => (string)this.GetValue(HelperTextProperty);
			set => this.SetValue(HelperTextProperty, value);
		}

		public float LabelTextHeight { get; private set; }

		public float PlaceholderTextHeight { get; private set; }

		public float LabelTextWidth { get; private set; }

		public float PlaceholderTextWidth { get; private set; }

		public double ParentHeightRequest { get; set; }

		public double ParentWidthRequest { get; set; }

		private bool _isInitialDraw = true;

		private static void LabelTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			entry.MeasureText((string) newvalue);
		}

		private static void ErrorColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = (Color) newvalue;
			
			entry.SkErrorColor = color.ToSKColor();
		}

		private static void ForegroundColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = (Color) newvalue;
			
			entry.SkForegroundColor = color.ToSKColor();
		}

		private static void TintColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = (Color) newvalue;
			
			entry.SkTintColor = color.ToSKColor();
		}

		private static object ForegroundColorDefaultValueCreator(BindableObject bindable)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = Color.FromHex("#FF008000");
			
			entry.SkForegroundColor = color.ToSKColor();
			
			return color;
		}

		private static object ErrorColorDefaultValueCreator(BindableObject bindable)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = Color.FromHex("#b00020");
			
			entry.SkErrorColor = color.ToSKColor();
			
			return color;
		}

		private static object TintColorDefaultValueCreator(BindableObject bindable)
		{
			InternalOutlineView entry = bindable as InternalOutlineView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = Color.FromHex("#6200ee");
			
			entry.SkTintColor = color.ToSKColor();
			
			return color;
		}

		internal void MeasureText(string text)
		{
			// measure the text for the clip path
			using (SKPaint paintText = new SKPaint())
			{
				paintText.TextSize = this.LabelFontSize;

				paintText.IsStroke = false;

				this.LabelTextWidth = paintText.MeasureText(text);

				this.LabelTextHeight = paintText.FontMetrics.XHeight * (float) Device.Info.ScalingFactor;

				paintText.TextSize = this.PlaceHolderFontSize;

				this.PlaceholderTextWidth = paintText.MeasureText(text);

				this.PlaceholderTextHeight = paintText.FontMetrics.XHeight * (float) Device.Info.ScalingFactor;
			}
		}

		public void Init()
		{
			_isInitialDraw = true;

			if (!this.HasText)
			{
				this._currentTextSize = this.PlaceHolderFontSize;

				float scaledHeightRequest = (float) (this.ParentHeightRequest * Device.Info.ScalingFactor);

				this._currentTextY = (scaledHeightRequest + this._currentTextSize / 2f) / 2f + this.PlaceholderTextHeight / 3f;
			}
			else
			{
				this._currentTextSize = this.LabelFontSize;

				this._currentTextY = this._currentTextSize / 2f + this.LabelTextHeight / 2f;
			}
		}

		/// <param name="e">The event arguments that contain the drawing surface and information.</param>
		/// <summary>Implement this to draw on the canvas.</summary>
		/// <remarks>
		///     <format type="text/markdown"><![CDATA[
		/// ## Remarks
		/// There are two ways to draw on this surface: by overriding the
		/// <xref:SkiaSharp.Views.Forms.SKCanvasView.OnPaintSurface(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs)>
		/// method, or by attaching a handler to the
		/// <xref:SkiaSharp.Views.Forms.SKCanvasView.PaintSurface>
		/// event.
		/// > [!IMPORTANT]
		/// > If this method is overridden, then the base must be called, otherwise the
		/// > event will not be fired.
		/// ## Examples
		/// ```csharp
		/// protected override void OnPaintSurface (SKPaintSurfaceEventArgs e)
		/// {
		///     // call the base method
		///     base.OnPaintSurface (e);
		///     var surface = e.Surface;
		///     var surfaceWidth = e.Info.Width;
		///     var surfaceHeight = e.Info.Height;
		///     var canvas = surface.Canvas;
		///     // draw on the canvas
		///     canvas.Flush ();
		/// }
		/// ```
		/// ]]></format>
		/// </remarks>
		protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			SKImageInfo info = e.Info;

			SKSurface surface = e.Surface;

			SKCanvas canvas = surface.Canvas;

			if (this.InternalIsFocused)
			{
				float textSize = this.LabelFontSize;

				float originTextSize = Math.Abs(this._currentTextSize - textSize) < 1f ? textSize : 20.0f;

				float textDestinationY = textSize / 2f + this.LabelTextHeight / 2f;

				float textOriginY = Math.Abs(this._currentTextY - textDestinationY) < 1f ? textDestinationY : (info.Height + textSize / 2f) / 2 + this.PlaceholderTextHeight / 3f;

				float time = (float) this.CurrentLoopIteration / this.TotalLoops;

				this.DrawFocused(info, canvas, 2f, this.SkTintColor);

				this.DrawText(canvas, originTextSize, textSize, this.SkTintColor, textDestinationY, textOriginY, time);
			}
			else
			{
				float time = (float) this.CurrentLoopIteration / this.TotalLoops;

				if(_isInitialDraw)
                {
					time = 1;
					_isInitialDraw = false;
                }

				if (!this.HasText)
				{
					float textSize = this.PlaceHolderFontSize;

					float originTextSize = Math.Abs(this._currentTextSize - textSize) < 1f ? textSize : 12.0f;

					float textDestinationY = (info.Height + textSize / 2f) / 2f + this.PlaceholderTextHeight / 3f;

					float textOriginY = Math.Abs(this._currentTextY - textDestinationY) < 1f ? textDestinationY : textSize / 2f + this.LabelTextHeight / 2f;

					this.DrawUnfocused(info, canvas, 1f, this.SkForegroundColor);

					this.DrawText(canvas, originTextSize, textSize, this.SkForegroundColor, textDestinationY, textOriginY, time);
				}
				else
				{
					float textSize = this.LabelFontSize;

					float originTextSize = Math.Abs(this._currentTextSize - textSize) < 1f ? textSize : 20.0f;

					float textDestinationY = textSize / 2f + this.LabelTextHeight / 2f;

					float textOriginY = Math.Abs(this._currentTextY - textDestinationY) < 1f ? textDestinationY : (info.Height + textSize / 2f) / 2 + this.PlaceholderTextHeight / 3f;

					this.DrawFocused(info, canvas, 1f, this.SkForegroundColor);

					this.DrawText(canvas, originTextSize, textSize, this.SkForegroundColor, textDestinationY, textOriginY, time);
				}

				// draw on the canvas
				canvas.Flush();
			}
		}

		private void DrawUnfocused(SKImageInfo info, SKCanvas canvas, float strokeWidth, SKColor colour)
		{
			canvas.Clear();

			using (SKPaint paint = new SKPaint())
			{
				paint.Style = SKPaintStyle.Stroke;

				paint.Color = colour;

				paint.IsAntialias = true;

				paint.StrokeWidth = strokeWidth;

				Bounds bounds = new Bounds
				{
					Left = strokeWidth,
					Right = info.Width - strokeWidth,
					Top = strokeWidth + this.LabelTextHeight / 2f,
					Bottom = info.Height - strokeWidth
				};

				this.DrawOutline(canvas, strokeWidth, bounds, paint, false);
			}
		}

		private void DrawOutline(SKCanvas canvas, float strokeWidth, Bounds bounds, SKPaint paint, bool cutoutForLabel)
		{
			using (SKPath path2 = new SKPath())
			{
				path2.MoveTo(this.CornerRadius, bounds.Top);

				if (cutoutForLabel && !string.IsNullOrWhiteSpace(this.Placeholder))
				{
					path2.LineTo(this.LabelMargin - this.LabelPadding, bounds.Top);

					path2.MoveTo(strokeWidth * 2 + this.LabelMargin + this.LabelPadding + this.LabelTextWidth, bounds.Top);
				}

				path2.LineTo(bounds.Right - this.CornerRadius, bounds.Top);

				path2.QuadTo(bounds.Right, bounds.Top, bounds.Right, this.CornerRadius + bounds.Top);

				path2.LineTo(bounds.Right, bounds.Bottom - this.CornerRadius);

				path2.QuadTo(bounds.Right, bounds.Bottom, bounds.Right - this.CornerRadius, bounds.Bottom);

				path2.LineTo(this.CornerRadius, bounds.Bottom);

				path2.QuadTo(bounds.Left, bounds.Bottom, bounds.Left, bounds.Bottom - this.CornerRadius);

				path2.LineTo(bounds.Left, this.CornerRadius + bounds.Top);

				path2.QuadTo(bounds.Left, bounds.Top, this.CornerRadius, bounds.Top);

				canvas.DrawPath(path2, paint);
			}
		}

		private void DrawText(SKCanvas canvas, float originTextSize, float textSize, SKColor textColour, float destinationY, float originY, float time)
		{
			if (string.IsNullOrWhiteSpace(this.Placeholder))
			{
			}
			else
			{
				// draw left-aligned text, solid
				using (SKPaint paintText = new SKPaint())
				{
					paintText.IsAntialias = true;

					paintText.Color = textColour;

					paintText.IsStroke = false;

					time = Easing.Smoothstep(time);

					this._currentTextY = destinationY * time + originY * (1 - time);

					this._currentTextSize = textSize * time + originTextSize * (1 - time);

					paintText.TextSize = this._currentTextSize;

					///SKShaper shaper = new SKShaper(SKTypeface.Default);

					//canvas.DrawShapedText(shaper, this.Placeholder, this.LabelMargin, this._currentTextY, paintText);
					canvas.DrawText(this.Placeholder, this.LabelMargin, this._currentTextY, paintText);
				}
			}
		}

		private void DrawFocused(SKImageInfo info, SKCanvas canvas, float strokeWidth, SKColor colour)
		{
			canvas.Clear();

			using (SKPaint paint = new SKPaint())
			{
				paint.Style = SKPaintStyle.Stroke;

				paint.Color = colour;

				paint.IsAntialias = true;

				paint.StrokeWidth = strokeWidth;

				Bounds bounds = new Bounds
				{
					Left = strokeWidth,
					Right = info.Width - strokeWidth,
					Top = strokeWidth + this.LabelTextHeight / 2f,
					Bottom = info.Height - strokeWidth
				};

				this.DrawOutline(canvas, strokeWidth, bounds, paint, true);
			}
		}
	}
}