using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XF.Material.Outline;

[assembly: ExportRenderer(typeof(InternalTextView), typeof(InternalTextViewRenderer))]

namespace XF.Material.Outline
{
	public class InternalTextViewRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (this.Control != null)
			{
				// do whatever you want to the UITextField here!
				this.Control.BackgroundColor = UIColor.Clear;
				this.Control.BorderStyle = UITextBorderStyle.None;

				if (e.NewElement != null)
				{
					InternalTextView materialOutlineTextView = e.NewElement as InternalTextView;

					this.Control.TintColor = materialOutlineTextView.TintColor.ToUIColor();
				}
			}
		}
	}
}