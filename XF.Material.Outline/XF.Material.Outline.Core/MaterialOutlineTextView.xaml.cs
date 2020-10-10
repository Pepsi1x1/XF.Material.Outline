using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace XF.Material.Outline.Core
{
	[Preserve (AllMembers = true)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MaterialOutlineTextView : ContentView
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.TwoWay);

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MaterialOutlineTextView), 12.0, BindingMode.OneWay);
				
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MaterialOutlineTextView), Color.Black, BindingMode.OneWay);

		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, LabelTextPropertyChanged);

		public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(nameof(HelperText), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.OneWay);//, null, HelperTextPropertyChanged);

		public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.OneWay);

		public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(nameof(HasError), typeof(bool), typeof(MaterialOutlineTextView), false, BindingMode.OneWay);

		public static readonly BindableProperty PlaceholderFontSizeProperty = BindableProperty.Create(nameof(PlaceholderFontSize), typeof(float), typeof(MaterialOutlineTextView), 14f, BindingMode.OneWay);

		public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create(nameof(LabelFontSize), typeof(float), typeof(MaterialOutlineTextView), 12f, BindingMode.OneWay);

		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, TintColorPropertyChanged, null, null, TintColorDefaultValueCreator);

		public static readonly BindableProperty ForegroundColorProperty =
			BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, ForegroundColorPropertyChanged, null, null, ForegroundColorDefaultValueCreator);

		public static readonly BindableProperty ErrorColorProperty =
			BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, ErrorColorPropertyChanged, null, null, ErrorColorDefaultValueCreator);

		public MaterialOutlineTextView()
		{
			this.InitializeComponent();

			this.TextView.Focused += this.TextViewOnFocused;
			this.TextView.Unfocused += this.TextViewOnFocused;

			this.TextView.TextChanged += this.OnTextChanged;
		}

		private bool _hasSupplementaryText = false;

		public bool HasError
		{
			get => (bool)this.GetValue(HasErrorProperty);
			set => this.SetValue(HasErrorProperty, value);
		}

		public double FontSize
		{
			get => (double)this.GetValue(FontSizeProperty);
			set => this.SetValue(FontSizeProperty, value);
		}

		public Color TextColor
		{
			get => (Color)this.GetValue(TextColorProperty);
			set => this.SetValue(TextColorProperty, value);
		}

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

		public string HelperText
		{
			get => (string) this.GetValue(HelperTextProperty);
			set => this.SetValue(HelperTextProperty, value);
		}

		public string ErrorText
		{
			get => (string)this.GetValue(ErrorTextProperty);
			set => this.SetValue(ErrorTextProperty, value);
		}

		public string Placeholder
		{
			get => (string) this.GetValue(PlaceholderProperty);
			set => this.SetValue(PlaceholderProperty, value);
		}

		public string Text
		{
			get => (string)this.GetValue(TextProperty);
			set => this.SetValue(TextProperty, value);
		}

		public float PlaceholderFontSize
		{
			get => (float)this.GetValue(PlaceholderFontSizeProperty);
			set => this.SetValue(PlaceholderFontSizeProperty, value);
		}

		public float LabelFontSize
		{
			get => (float)this.GetValue(LabelFontSizeProperty);
			set => this.SetValue(LabelFontSizeProperty, value);
		}

		public static CancellationTokenSource AnimCancellationToken { get; set; }

		private static void AdjustHeightForSupplementaryText(MaterialOutlineTextView entry, bool grow)
        {
			var grid = entry.Content as Grid;
			if (grow)
			{
				grid.RowDefinitions[2].Height = 16;
				Grid.SetRowSpan(entry.TextView, 1);
			}
			else
			{
				grid.RowDefinitions[2].Height = 0;
				Grid.SetRowSpan(entry.TextView, 2);
			}
		}

		private static void LabelTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			entry.OutlineView.MeasureText((string) newvalue);

			entry.OutlineView.SetValue(InternalOutlineView.PlaceholderProperty, newvalue);

			entry.OutlineView.HasText = !string.IsNullOrWhiteSpace(entry.TextView.Text);

			entry.OutlineView.Init();

			entry.OutlineView.InvalidateSurface();
		}

		private static void ErrorColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = (Color) newvalue;

			entry.OutlineView.ErrorColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.ErrorColorProperty, color);
		}

		private static void ForegroundColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = (Color) newvalue;

			entry.OutlineView.ForegroundColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.ForegroundColorProperty, color);
		}

		private static void TintColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = (Color) newvalue;

			entry.TextView.TintColor = color;

			entry.OutlineView.TintColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.TintColorProperty, color);
		}
		
		private static object ErrorColorDefaultValueCreator(BindableObject bindable)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = Color.FromHex("#b00020");

			entry.OutlineView.ErrorColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.ErrorColorProperty, color);

			return color;
		}

		private static object ForegroundColorDefaultValueCreator(BindableObject bindable)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = Color.FromHex("#FF008000");

			entry.OutlineView.ForegroundColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.ForegroundColorProperty, color);

			return color;
		}

		private static object TintColorDefaultValueCreator(BindableObject bindable)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			Color color = Color.FromHex("#6200ee");

			entry.TextView.TintColor = color;

			entry.OutlineView.TintColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.TintColorProperty, color);

			return color;
		}

		public void TapGestureRecognizer_Tapped(object sender, EventArgs args)
        {
			this.TextView.Focus();
        }

		/// <param name="propertyName">The name of the bound property that changed.</param>
		/// <summary>Method that is called when a bound property is changed.</summary>
		/// <remarks>To be added.</remarks>
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(HeightRequest):
                    OutlineView.ParentHeightRequest = HeightRequest;
                    break;
                case nameof(Text):
                    TextView.Text = Text;
                    break;
                case nameof(WidthRequest):
                    OutlineView.ParentWidthRequest = WidthRequest;
                    break;
                case nameof(FontSize):
                    TextView.FontSize = FontSize;
                    break;
                case nameof(ErrorText):
                    OutlineView.ErrorText = ErrorText;

                    UpdateHasSupplementaryText();

                    AdjustHeightForSupplementaryText(this, _hasSupplementaryText);
                    break;
                case nameof(TextColor):
                    TextView.TextColor = TextColor;
                    break;
                case nameof(HasError):
                    OutlineView.HasError = HasError;

                    UpdateHasSupplementaryText();

                    AdjustHeightForSupplementaryText(this, _hasSupplementaryText);
                    break;
                case nameof(HelperText):
                    OutlineView.HelperText = HelperText;

                    UpdateHasSupplementaryText();

                    AdjustHeightForSupplementaryText(this, _hasSupplementaryText);
                    break;
                case nameof(PlaceholderFontSize):
                    OutlineView.PlaceholderFontSize = PlaceholderFontSize;
                    break;
                case nameof(LabelFontSize):
                    OutlineView.LabelFontSize = LabelFontSize;
                    break;
            }
        }

		private void UpdateHasSupplementaryText()
        {
			if (this.HasError && !string.IsNullOrWhiteSpace(this.ErrorText))
			{
				this._hasSupplementaryText = true;
				return;
			}

			if (!string.IsNullOrWhiteSpace(this.HelperText))
			{
				this._hasSupplementaryText = true;
				return;
			}

			this._hasSupplementaryText = false;
		}

        private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			this.OutlineView.HasText = !string.IsNullOrWhiteSpace(e.NewTextValue);
		}

		public void TextViewOnFocused(object sender, FocusEventArgs e)
		{
			InternalOutlineView control = this.OutlineView;

			control.InternalIsFocused = e.IsFocused;

			control.CurrentLoopIteration = 0;

			Task.Run(this.RunAnimationAsync);
		}

		private Task RunAnimationAsync()
		{
			InternalOutlineView control = this.OutlineView;

			AnimCancellationToken = new CancellationTokenSource ();
			while (!AnimCancellationToken.IsCancellationRequested)
			{
				control.CurrentLoopIteration++;

				if (control.TotalLoops == control.CurrentLoopIteration)
				{
					return Task.CompletedTask;
				}

				Device.BeginInvokeOnMainThread(control.InvalidateSurface);
			}
			return Task.CompletedTask;
		}
    }
}