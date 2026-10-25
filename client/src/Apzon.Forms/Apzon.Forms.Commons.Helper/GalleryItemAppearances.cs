using DevExpress.Utils;
using DevExpress.XtraBars;

namespace Apzon.Forms.Commons.Helper;

internal static class GalleryItemAppearances
{
	public static void Apply(RibbonGalleryBarItem galleryItem)
	{
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Normal.Font = AppearanceObject.DefaultFont;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseFont = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Near;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseTextOptions = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Hovered.Font = AppearanceObject.DefaultFont;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseFont = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Hovered.TextOptions.HAlignment = HorzAlignment.Near;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseTextOptions = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Pressed.Font = AppearanceObject.DefaultFont;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseFont = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Pressed.TextOptions.HAlignment = HorzAlignment.Near;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseTextOptions = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Disabled.Font = AppearanceObject.DefaultFont;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Disabled.Options.UseFont = true;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Disabled.TextOptions.HAlignment = HorzAlignment.Near;
		galleryItem.Gallery.Appearance.ItemCaptionAppearance.Disabled.Options.UseTextOptions = true;
	}
}
