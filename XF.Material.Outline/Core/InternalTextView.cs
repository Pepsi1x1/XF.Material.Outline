using Xamarin.Forms;
using XF.Material.Outline.Core;

namespace XF.Material.Outline
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