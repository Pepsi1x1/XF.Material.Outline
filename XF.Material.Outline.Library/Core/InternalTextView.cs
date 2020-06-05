using Xamarin.Forms;

namespace XF.Material.Outline.Core
{
	public class InternalTextView : Entry
	{
		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(InternalOutlineView), Color.FromHex("#6200ee"));

		public Color TintColor
		{
			get => (Color) this.GetValue(TintColorProperty);
			set => this.SetValue(TintColorProperty, value);
		}
	}
}