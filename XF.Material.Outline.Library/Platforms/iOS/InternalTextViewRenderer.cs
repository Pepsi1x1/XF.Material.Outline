using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(XF.Material.Outline.Core.InternalTextView), typeof(XF.Material.Outline.iOS.InternalTextViewRenderer))]
namespace XF.Material.Outline.iOS
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
					XF.Material.Outline.Core.InternalTextView materialOutlineTextView = e.NewElement as XF.Material.Outline.Core.InternalTextView;

					this.Control.TintColor = materialOutlineTextView.TintColor.ToUIColor();
				}
			}
		}
	}
}