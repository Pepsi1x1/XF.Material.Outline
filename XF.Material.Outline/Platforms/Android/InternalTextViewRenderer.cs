using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.Material.Outline;

[assembly: ExportRenderer(typeof(InternalTextView), typeof(InternalTextViewRenderer))]

namespace XF.Material.Outline
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
					InternalTextView textView = e.NewElement as InternalTextView;

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

			this.Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
			this.Control.SetPadding(0, 0, 0, 0);
			this.Control.SetIncludeFontPadding(false);
		}

		private void ChangeCursorColor(Android.Graphics.Color color)
		{
			if (this.Control == null)
			{
				return;
			}

			try
			{
				var cursorResource = Java.Lang.Class.FromType(typeof(Android.Widget.TextView)).GetDeclaredField(CURSOR_DRAWABLE_RESOURCE);

				cursorResource.Accessible = true;

				int resId = cursorResource.GetInt(this.Control);

				Java.Lang.Reflect.Field editorField = Java.Lang.Class.FromType(typeof(Android.Widget.TextView)).GetDeclaredField(EDITOR_FIELD);

				editorField.Accessible = true;

				Android.Graphics.Drawables.Drawable cursorDrawable = Context.GetDrawable(resId);

				cursorDrawable.SetColorFilter(color, Android.Graphics.PorterDuff.Mode.SrcIn);

				Java.Lang.Object editor = editorField.Get(this.Control);

				Java.Lang.Reflect.Field cursorDrawableField = editor.Class.GetDeclaredField(CUSROR_DRAWABLE_FIELD);

				cursorDrawableField.Accessible = true;

				cursorDrawableField.Set(editor, new[] {cursorDrawable, cursorDrawable});
			}
			catch (Java.Lang.NoSuchFieldException)
			{
				// Whoops chaning the cursor color is not supported :(
			}
			catch (Android.Content.Res.Resources.NotFoundException)
			{
				// Uh oh, no cursor drawable!
			}
		}
	}
}