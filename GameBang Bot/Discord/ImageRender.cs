using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameBang_Bot.Discord {
	class ImageRender {
		private const string DIR = "images";
		private const string BROKEN = "broken";

		public static MemoryStream GetMatchImageStream(string team1, string team2) {
			var background = new Bitmap($"{DIR}/background.png");
			var graphic = Graphics.FromImage(background);

			using var logo1 = new Bitmap($"{DIR}/{team1}.png");
			using var logo2 = new Bitmap($"{DIR}/{team2}.png");
			using var rlogo1 = new Bitmap(logo1, TargetSize(logo1.Width, logo1.Height, 350));
			using var rlogo2 = new Bitmap(logo2, TargetSize(logo2.Width, logo2.Height, 350));

			graphic.DrawImage(rlogo1, TargetPoint(rlogo1.Width, rlogo1.Height, 228, 274));
			graphic.DrawImage(rlogo2, TargetPoint(rlogo2.Width, rlogo2.Height, 760, 274));
			graphic.Save();

			var ms = new MemoryStream();
			background.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			ms.Seek(0, SeekOrigin.Begin);

			return ms;
		}

		public static MemoryStream GetOverImageStream(MemoryStream ms, bool isRedWon) {
			var background = new Bitmap(ms);
			var graphic = Graphics.FromImage(background);

			using var logo = new Bitmap($"{DIR}/{BROKEN}.png");

			if(isRedWon)
				graphic.DrawImage(logo, TargetPoint(logo.Width, logo.Height, 228, 274));
			else
				graphic.DrawImage(logo, TargetPoint(logo.Width, logo.Height, 760, 274));
			graphic.Save();

			var ms2 = new MemoryStream();
			background.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
			ms2.Seek(0, SeekOrigin.Begin);
			ms.Close();
			return ms2;
		}

		private static Size TargetSize(int width, int height, int target) {
			var ratio = height / (double)width;

			if(height > width) {
				height = target;
				width = (int)(target / ratio);
			} else {
				width = target;
				height = (int)(target * ratio);
			}

			return new Size(width, height);
		}


		private static Point TargetPoint(int width, int height, int tWidth, int tHeight) {
			return new Point(tWidth - (width / 2), tHeight - (height / 2));
		}

	}
}
