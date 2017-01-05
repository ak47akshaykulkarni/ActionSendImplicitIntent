using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using System;
using Android.Views;

namespace ShareTextDroid
{
    [IntentFilter(new string[] { Intent.ActionSend },
    Categories = new string[] { Intent.CategoryDefault },
    DataMimeType = "image/*")]
    [IntentFilter(new string[] { Intent.ActionSendMultiple },
    Categories = new string[] { Intent.CategoryDefault },
    DataMimeType = "image/*")]
    [IntentFilter(new string[] { Intent.ActionView },
    Categories = new string[] { Intent.CategoryDefault },
    DataMimeType = "image/*")]

    [Activity(Label = "Shared Image", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button btn;
        ImageView gotImage;
        SeekBar brseek;
        Bitmap bmp, operation;
        TextView converted;
        int upp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            
            btn = FindViewById<Button>(Resource.Id.asd);
            brseek = FindViewById<SeekBar>(Resource.Id.seekBar1);
            gotImage = FindViewById<ImageView>(Resource.Id.gotImage);
            converted = FindViewById<TextView>(Resource.Id.converted);
            
            if (Intent.Action == Intent.ActionSend)
            {
                Bundle extr = Intent.Extras;
                Java.Lang.Object obj = extr.Get(Intent.ExtraStream);
                Android.Net.Uri img = Android.Net.Uri.Parse(obj.ToString());
                gotImage.SetImageURI(img);

                gotImage.BuildDrawingCache(true);

                Bitmap bitmap = gotImage.GetDrawingCache(true);

                BitmapDrawable drawable = (BitmapDrawable)gotImage.Drawable;
                Bitmap bitamap = drawable.Bitmap;
                bmp = bitamap;
                btn.Click += (s, args) =>
                {
                    using (var stream = new MemoryStream())
                    {
                        bitamap.Compress(Bitmap.CompressFormat.Png, 0, stream);

                        var bytes = stream.ToArray();
                        var str = Convert.ToBase64String(bytes);

                        Toast.MakeText(this, "Base64 Converted", ToastLength.Short).Show();

                        converted.Text = str;
                        
                    }
                };
                btn.LongClick += (s, args) =>
                {
                        green(gotImage);   
                };
                brseek.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
                    if (e.FromUser)
                    {
                        gotImage.SetColorFilter(brightIt(Convert.ToInt32(e.Progress)));//foto is my ImageView
                        upp = e.Progress;
                        //btn.Text = string.Format("The value of the SeekBar is {0}", e.Progress);
                    }
                };
                //Log.Debug("Share", Intent.Type);
            }
            else if (Intent.Action == Intent.ActionSendMultiple)
            {
                btn.Text = "getting images";
                //Log.Debug("Share", Intent.Type);
            }
            else if (Intent.ActionView.Equals(Intent.Action) && Intent.Type != null)
            {

                if (Intent.Type.StartsWith("image/"))
                {

                    gotImage.SetImageURI(Intent.Data);
                    //handleSendImage(intent); // Handle single image being sent

                }

            }
            else
            {
                Bundle extr = Intent.Extras;
            }
        }

        private ColorMatrixColorFilter brightIt(int fb)
        {
            ColorMatrix cmB = new ColorMatrix();
            cmB.Set(new float[] {
            1, 0, 0, 0, fb,
            0, 1, 0, 0, fb,
            0, 0, 1, 0, fb,
            0, 0, 0, 1, 0   });
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Set(cmB);
            //Canvas c = new Canvas(b2);
            //Paint paint = new Paint();
            ColorMatrixColorFilter f = new ColorMatrixColorFilter(colorMatrix);
            //paint.setColorFilter(f);   
            return f;
        }

        public void green(View view)
        {
            operation = Bitmap.CreateBitmap(bmp.Width, bmp.Height, bmp.GetConfig());

            for (int i = 0; i< bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    
                    int p = bmp.GetPixel(i, j);
                    
                    int r = Color.GetRedComponent(p);
                    int g = Color.GetGreenComponent(p);
                    int b = Color.GetBlueComponent(p);
                    int alpha = Color.GetAlphaComponent(p);

                    r = 0+upp+10;
                    g = g + upp;
                    b = 0+upp -20;
                    alpha = 0;
                    operation.SetPixel(i, j, Color.Argb(Color.GetAlphaComponent(p), r, g, b));
                }
            }
            gotImage.SetImageBitmap(operation);
        }
    }
}