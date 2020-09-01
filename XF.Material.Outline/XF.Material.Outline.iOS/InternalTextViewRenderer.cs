using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(XF.Material.Outline.Core.InternalTextView), typeof(XF.Material.Outline.iOS.InternalTextViewRenderer))]
namespace XF.Material.Outline.iOS
{
	//[Preserve (AllMembers = true)]
	public class InternalTextViewRenderer : EntryRenderer
	{
		public InternalTextViewRenderer() : base()
		{

		}

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
					XF.Material.Outline.Core.InternalTextView textView = e.NewElement as XF.Material.Outline.Core.InternalTextView;
					if (textView == null)
					{
						throw new ArgumentNullException(nameof(textView));
					}

					this.Control.TintColor = textView.TintColor.ToUIColor();
				}
			}
		}
	}
}