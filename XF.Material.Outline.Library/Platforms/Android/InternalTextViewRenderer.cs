using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(XF.Material.Outline.Core.InternalTextView), typeof(XF.Material.Outline.Android.InternalTextViewRenderer))]
namespace XF.Material.Outline.Android
{
	public class InternalTextViewRenderer : EntryRenderer
	{
		private const string CURSOR_DRAWABLE_RESOURCE = "mCursorDrawableRes";
		private const string CUSROR_DRAWABLE_FIELD = "mCursorDrawable";
		private const string EDITOR_FIELD = "mEditor";

		public InternalTextViewRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (this.Control != null)
			{
				this.SetControl();

				if (e.NewElement != null)
				{
					XF.Material.Outline.Core.InternalTextView textView = e.NewElement as XF.Material.Outline.Core.InternalTextView;

					this.ChangeCursorColor(textView.TintColor.ToAndroid());
				}
			}
		}

		private void SetControl()
		{
			if(this.Control == null)
			{
				return;
			}

			this.Control.SetBackgroundColor(Color.Transparent);
			this.Control.SetPadding(0, 0, 0, 0);
			this.Control.SetIncludeFontPadding(false);
		}

		private void ChangeCursorColor(Color color)
		{
			if (this.Control == null)
			{
				return;
			}

			try
			{
				var cursorResource = Java.Lang.Class.FromType(typeof(TextView)).GetDeclaredField(CURSOR_DRAWABLE_RESOURCE);

				cursorResource.Accessible = true;

				int resId = cursorResource.GetInt(this.Control);

				Java.Lang.Reflect.Field editorField = Java.Lang.Class.FromType(typeof(TextView)).GetDeclaredField(EDITOR_FIELD);

				editorField.Accessible = true;

				Drawable cursorDrawable = Context.GetDrawable(resId);

				cursorDrawable.SetColorFilter(new BlendModeColorFilter(color, BlendMode.SrcIn));

				Java.Lang.Object editor = editorField.Get(this.Control);

				Java.Lang.Reflect.Field cursorDrawableField = editor.Class.GetDeclaredField(CUSROR_DRAWABLE_FIELD);

				cursorDrawableField.Accessible = true;

				cursorDrawableField.Set(editor, new[] {cursorDrawable, cursorDrawable});
			}
			catch (Exception)
			{
				// Whoops chaning the cursor color is not supported :(
			}
		}
	}
}