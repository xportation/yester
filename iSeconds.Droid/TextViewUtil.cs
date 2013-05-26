using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using System.ComponentModel;
using Android.Graphics;

namespace iSeconds.Droid
{
   internal class TextViewUtil	
	{
		static private void ChangeFont(TextView textView, Context context, string fontName, float fontSize)
		{
			Typeface tf = Typeface.CreateFromAsset(context.Assets, fontName);
			textView.SetTypeface(tf,TypefaceStyle.Normal);
			textView.TextSize= fontSize;
		}

		static public void ChangeFontForActionBarTitle(TextView textView, Context context, float fontSize)
		{
			ChangeFont(textView,context,"fonts/augie.ttf",fontSize);
		}
		
		public static void ChangeForDefaultFont(TextView textView, Context context, float fontSize)
		{
			ChangeFont(textView,context,"fonts/123Marker.ttf",fontSize);
		}
	}
   
}
