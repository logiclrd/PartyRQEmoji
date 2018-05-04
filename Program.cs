using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PartyRQEmoji
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var logo = new RQLogo();

			logo.Width = 35;
			logo.Height = 25;
			logo.HorizontalAlignment = HorizontalAlignment.Center;
			logo.VerticalAlignment = VerticalAlignment.Center;

			const double CenterX = 17.5;
			const double CenterY = 25;

			var skew = new SkewTransform(0, 0, CenterX, CenterY);
			var stretch = new ScaleTransform(1.0, 1.0, CenterX, CenterY);

			var transforms = new TransformGroup();

			transforms.Children.Add(skew);
			transforms.Children.Add(stretch);

			logo.RenderTransform = transforms;

			/*
			var grid = new Grid();

			grid.HorizontalAlignment = HorizontalAlignment.Center;
			grid.VerticalAlignment = VerticalAlignment.Center;

			var referenceLogo = new RQLogo();

			referenceLogo.Width = logo.Width;
			referenceLogo.Height = logo.Height;
			referenceLogo.Background = Brushes.Red;

			grid.Children.Add(referenceLogo);
			grid.Children.Add(logo);

			var window =
				new Window()
				{
					Content = grid
				};

			window.MouseMove +=
				(sender, e) =>
				{
					var mousePosition = e.GetPosition(referenceLogo);

					// mouse position is now relative to top/left
					mousePosition.X = mousePosition.X - CenterX;
					mousePosition.Y = mousePosition.Y - CenterY;

					skew.AngleX = Math.Atan2(mousePosition.X, mousePosition.Y) * 57.295779;
					stretch.ScaleY = -mousePosition.Y / CenterY;

					Console.WriteLine(skew.AngleX);
				};
			*/
			var window =
				new Window()
				{
					Content = logo
				};

			var app = new Application();

			var whiteFill = new Rectangle();

			whiteFill.Fill = Brushes.White;
			whiteFill.HorizontalAlignment = HorizontalAlignment.Stretch;
			whiteFill.VerticalAlignment = VerticalAlignment.Stretch;

			Task.Run(
				() =>
				{
					int index = 0;

					while (true)
					{
						Thread.Sleep(200);

						app.Dispatcher.Invoke(
							() =>
							{
								var bitmap = new RenderTargetBitmap(50, 50, 96.0, 96.0, PixelFormats.Pbgra32);

								whiteFill.Measure(new Size(bitmap.PixelWidth, bitmap.PixelHeight));
								whiteFill.Arrange(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

								logo.Measure(new Size(bitmap.PixelWidth, bitmap.PixelHeight));
								logo.Arrange(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

								bitmap.Render(whiteFill);
								bitmap.Render(logo);

								var pngEncoder = new PngBitmapEncoder();

								pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));

								using (var stream = File.OpenWrite("Frame" + index.ToString("d2") + ".png"))
									pngEncoder.Save(stream);

								index = (index + 1) % Points.Length;

								var point = Points[index];

								logo.Foreground = new SolidColorBrush(Color.FromRgb(point.R, point.G, point.B));

								double x = point.X - CenterX;
								double y = point.Y - CenterY;

								skew.AngleX = Math.Atan2(x, y) * 57.295779;
								stretch.ScaleY = 2 * -y / CenterY;
							});
					}
				});

			app.Run(window);
		}

		class AnimationPoint
		{
			public byte R;
			public byte G;
			public byte B;
			public int X;
			public int Y;
		}

		static AnimationPoint[] Points =
			new AnimationPoint[]
			{
				new AnimationPoint() { R = 255, G = 141, B = 141, X = 23, Y = 6 },
				new AnimationPoint() { R = 255, G = 215, B = 137, X = 19, Y = 5 },
				new AnimationPoint() { R = 0, G = 255, B = 133, X = 15, Y = 6 },
				new AnimationPoint() { R = 0, G = 255, B = 255, X = 12, Y = 8 },
				new AnimationPoint() { R = 120, G = 182, B = 255, X = 11, Y = 9 },
				new AnimationPoint() { R = 237, G = 141, B = 255, X = 13, Y = 10 },
				new AnimationPoint() { R = 255, G = 141, B = 255, X = 18, Y = 11 },
				new AnimationPoint() { R = 255, G = 107, B = 251, X = 22, Y = 11 },
				new AnimationPoint() { R = 255, G = 107, B = 185, X = 25, Y = 9 },
				new AnimationPoint() { R = 255, G = 107, B = 107, X = 26, Y = 8 },
			};
	}
}
