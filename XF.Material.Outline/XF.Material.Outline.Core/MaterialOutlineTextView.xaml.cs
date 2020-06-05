using System;
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
		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, LabelTextPropertyChanged);

		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(MaterialOutlineTextView), null, BindingMode.OneWay, null, TintColorPropertyChanged, null, null, TintColorDefaultValueCreator);

		public MaterialOutlineTextView()
		{
			this.InitializeComponent();

			this.TextView.Focused += this.TextViewOnFocused;
			this.TextView.Unfocused += this.TextViewOnFocused;

			this.TextView.TextChanged += this.OnTextChanged;
		}

		public Color TintColor
		{
			get => (Color) this.GetValue(TintColorProperty);
			set => this.SetValue(TintColorProperty, value);
		}

		public string Placeholder
		{
			get => (string) this.GetValue(PlaceholderProperty);
			set => this.SetValue(PlaceholderProperty, value);
		}

		public static CancellationTokenSource AnimCancellationToken { get; set; }

		private static void LabelTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			entry.OutlineView.MeasureText((string) newvalue);

			entry.OutlineView.SetValue(InternalOutlineView.PlaceholderProperty, newvalue);

			entry.OutlineView.HasText = !string.IsNullOrWhiteSpace(entry.TextView.Text);

			entry.OutlineView.Init();

			entry.OutlineView.InvalidateSurface();
		}

		private static void TintColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

			Color color = (Color) newvalue;

			entry.TextView.TintColor = color;

			entry.OutlineView.TintColor = color;

			entry.OutlineView.SetValue(InternalOutlineView.TintColorProperty, color);
		}

		private static object TintColorDefaultValueCreator(BindableObject bindable)
		{
			MaterialOutlineTextView entry = bindable as MaterialOutlineTextView;

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
				this.OutlineView.ParentHeightRequest = this.HeightRequest;
			}
			else if (propertyName == nameof(this.WidthRequest))
			{
				this.OutlineView.ParentWidthRequest = this.WidthRequest;
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

				control.InvalidateSurface();
			}
			return Task.CompletedTask;
		}
	}
}