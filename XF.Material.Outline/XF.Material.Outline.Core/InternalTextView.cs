using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace XF.Material.Outline.Core
{
	[Preserve (AllMembers = true)]
	public class InternalTextView : Entry
	{
		public static readonly BindableProperty TintColorProperty =
			BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(InternalTextView), Color.FromHex("#6200ee"));

		public Color TintColor
		{
			get => (Color) this.GetValue(TintColorProperty);
			set => this.SetValue(TintColorProperty, value);
		}
	}
}