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

		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, LabelTextPropertyChanged);

		public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(nameof(HelperText), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, HelperTextPropertyChanged);

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

		public static CancellationTokenSource AnimCancellationToken { get; set; }
		
		private static void HelperTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Debug.Assert(entry != null, nameof(entry) + " != null");

			//if (string.IsNullOrWhiteSpace((string) oldvalue) && !string.IsNullOrWhiteSpace((string) newvalue))
			//{
			//	entry.HeightRequest += 16;
			//	Grid.SetRowSpan(entry.TextView, 1);
			//}
			//else if (string.IsNullOrWhiteSpace((string) newvalue) && !string.IsNullOrWhiteSpace((string) oldvalue))
			//{
			//	entry.HeightRequest -= 16;
			//	Grid.SetRowSpan(entry.TextView, 2);
			//}
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

		/// <param name="propertyName">The name of the bound property that changed.</param>
		/// <summary>Method that is called when a bound property is changed.</summary>
		/// <remarks>To be added.</remarks>
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == nameof(this.HeightRequest))
            {
                SetHeightForHelperText();
            }
            else if (propertyName == nameof(this.HelperText))
            {
                SetHeightForHelperText();

				this.OutlineView.HelperText = this.HelperText;
            }
			else if (propertyName == nameof(this.Text))
            {
				this.TextView.Text = this.Text;
            }
			else if (propertyName == nameof(this.WidthRequest))
			{
				this.OutlineView.ParentWidthRequest = this.WidthRequest;
			}
		}

        private void SetHeightForHelperText()
        {
            if (this.OutlineView.ParentHeightRequest != this.HeightRequest)
            {
                if (string.IsNullOrWhiteSpace(this.HelperText))
                {
                    this.OutlineView.ParentHeightRequest = this.HeightRequest += 16;
                    Grid.SetRowSpan(this.TextView, 1);
                }
                else
                {
                    this.OutlineView.ParentHeightRequest = this.HeightRequest -= 16;
                    Grid.SetRowSpan(this.TextView, 2);
                }
            }
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