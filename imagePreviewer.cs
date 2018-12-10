using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace ImagePreviewer
{
   	public partial class ImagePreviewer : Form
        {
		PictureBox pb;
		String[] arguments;
		int invert = 0;
	 	int resize = 0;
	 	int transparent = 0;
	 	float trans_value = 0.5F;
		int pb_width;
		int pb_height;
		Image normal;
		Image inverted;
		Image trans_image_normal;
		Image trans_image_inverted;
		int invert_t = 0;
		
		[DllImport("kernel32.dll")]
 		 [return: MarshalAs(UnmanagedType.Bool)]
 		 static extern bool AllocConsole();
 		[System.Runtime.InteropServices.DllImport("user32.dll")]
		 private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
		 
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		 private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		Dictionary<string, dynamic> image_types = new Dictionary<string, dynamic>();
		
		enum KeyModifier
		{
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			WinKey = 8
		}

		int id = 0;

		public ImagePreviewer(String[] args)
		{
			this.arguments = args;
			InitializeComponent();
		
			this.StartPosition = FormStartPosition.CenterScreen;
			
			RegisterHotKey(this.Handle, id, (int)KeyModifier.Shift, Keys.Space.GetHashCode());
			
			if (this.arguments.Length <=1)
			{
				AllocConsole();
				Console.WriteLine("Invalid inputs supplied");
				Console.WriteLine("Syntax: " + this.arguments[0] + " [image path] [Size in WxH Format | Full] [[Invert | NoInvert]] [[ Trans [Opacity] ]]");
				Console.Write("Press a key to continue...");
				Console.Read();
				this.Close();
				Application.Exit();
			}
			
			try
			{
				if (this.arguments.Length >= 3)
				{
					if (Regex.IsMatch(this.arguments[2], ".*x.*"))
					{
						this.resize = 1;
					}
					else if (this.arguments[2] == "Full")
					{
						this.resize = 2;
					}
					else
					{
						this.resize = 3;
					}
				}
				
				if (this.arguments.Length >= 4)
				{
					if (this.arguments[3] == "Invert")
					{
						this.invert = 1;
					}
					else if (this.arguments[3] == "NoInvert")
					{
						this.invert = 2;
					}
					else
					{
						this.invert = 3;
					}
				}
				if (this.arguments.Length >= 6)
				{
					if (this.arguments[4] == "Trans")
					{
						this.transparent = 1;
						this.trans_value = float.Parse(this.arguments[5]);
					}
				}
			} catch {
				AllocConsole();
				Console.WriteLine("Invalid inputs supplied");
				Console.WriteLine("Syntax: " + this.arguments[0] + "<Size in WxH Format | Full> [Invert | NoInvert] [Trans <Opacity>]");
				Console.Write("Press a key to continue...");
				Console.Read();
				this.Close();
				Application.Exit();
			}
			
			if ((this.invert == 0) && (this.transparent == 0) && (this.resize == 0))
			{
				AllocConsole();
				Console.WriteLine("Invalid inputs supplied");
				Console.WriteLine("Syntax: " + this.arguments[0] + "<Size in WxH Format | Full> [Invert | NoInvert] [Trans <Opacity>]");
				Console.Write("Press a key to continue...");
				Console.Read();
				this.Close();
				Application.Exit();
			}				

			if ((this.invert == 3) || (this.resize == 3))
			{
				AllocConsole();
				Console.WriteLine("Invalid inputs supplied");
				Console.WriteLine("Syntax: " + this.arguments[0] + "<Size in WxH Format | Full> [Invert | NoInvert] [Trans <Opacity>]");
				Console.Write("Press a key to continue...");
				Console.Read();
				this.Close();
				Application.Exit();
			}				

			this.normal = Image.FromFile(this.arguments[1]);
			this.inverted = InvertImage(this.normal);
			this.trans_image_normal = (Image) ChangeOpacity(this.normal, this.trans_value);
			this.trans_image_inverted = (Image) ChangeOpacity(this.inverted, this.trans_value);
			this.pb.Image = this.normal;

			this.invert_t = this.invert;
			if (this.resize == 1)
			{
				try
				{
					
					String size_str = this.arguments[2];
					string[] wAndH = size_str.Split('x');
				
					this.pb_width = Convert.ToInt32(wAndH[0]);
					this.pb_height = Convert.ToInt32(wAndH[1]);
					
					this.pb.Width = this.pb_width;
					this.pb.Height = this.pb_height;
				}
				catch
				{
				}
			}
			
			if (this.invert == 1)
			{
				this.pb.Image = this.inverted;
			}

			if (this.transparent == 1)
			{
				if (this.invert == 1)
				{
					this.pb.Image = this.trans_image_inverted;
				}
				else
				{
					this.pb.Image = this.trans_image_normal;
				}
			}

			this.ClientSize = new System.Drawing.Size(this.pb.Width, this.pb.Height);
			this.FormBorderStyle = FormBorderStyle.None;
			
			Bitmap pbImage = new Bitmap(this.pb.Image, this.pb.Width, this.pb.Height);
			this.pb.SizeMode = PictureBoxSizeMode.Zoom;
			this.pb.BorderStyle = BorderStyle.None;
			this.pb.Image = pbImage;
			
			this.KeyPreview = true;

			this.pb.MouseClick += Mouse_Click;

			this.Show();

		}
		
		private void Mouse_Click(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
		            	{
		            		this.Close();
		            		Application.Exit();
		            	}
		}
		/*
		 * Unregister HotKey when form closes
		 *
		 */
		private void ImagePreviewer_FormClosing(object sender, FormClosingEventArgs e)
		{
		    UnregisterHotKey(this.Handle, 0);       // Unregister hotkey with id 0 before closing the form. You might want to call this more than once with different id values if you are planning to register more than one hotkey.
		}
		
		private void ImagePreviewer_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)0x69)
			{
				if (this.invert_t == 0)
				{
					this.pb.Image = this.trans_image_inverted;
					this.invert_t = 1;
				}
				else
				{
					this.pb.Image = this.trans_image_normal;
					this.invert_t = 0;
				}
					
			}
			if (e.KeyChar == (char)0x2b)
			{
				if (this.invert_t == 0)
				{
					this.trans_value = this.trans_value + 0.1F;
					this.trans_image_normal = (Image) ChangeOpacity(this.normal, this.trans_value);
					this.pb.Image = this.trans_image_normal;
				}
				else
				{
					this.trans_value = this.trans_value + 0.1F;
					this.trans_image_inverted = (Image) ChangeOpacity(this.inverted, this.trans_value);
					this.pb.Image = this.trans_image_inverted;
				}				
			}
			if (e.KeyChar == (char)0x2d)
			{
				if (this.invert_t == 0)
				{
					this.trans_value = this.trans_value - 0.1F;
					this.trans_image_normal = (Image) ChangeOpacity(this.normal, this.trans_value);
					this.pb.Image = this.trans_image_normal;
				}
				else
				{
					this.trans_value = this.trans_value - 0.1F;
					this.trans_image_inverted = (Image) ChangeOpacity(this.inverted, this.trans_value);
					this.pb.Image = this.trans_image_inverted;
				}				
			}
			
			if (e.KeyChar == (char)0x73)
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();      
				saveFileDialog1.InitialDirectory = @"G:\"; 
				saveFileDialog1.Title = "Save File"; 
				saveFileDialog1.CheckFileExists = true; 
				saveFileDialog1.CheckPathExists = true; 
				saveFileDialog1.ShowHelp = true;
				saveFileDialog1.FilterIndex = 2; 
				saveFileDialog1.RestoreDirectory = true;      
				saveFileDialog1.CheckFileExists = false;
				saveFileDialog1.FilterIndex = 1;
				saveFileDialog1.OverwritePrompt = false;
				if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
				{ 
					string [] ext_array = saveFileDialog1.FileName.Split('.');
					string ext = ext_array[ext_array.Length - 1];

					if (this.invert_t == 1)
					{
						this.trans_image_inverted.Save(saveFileDialog1.FileName, this.image_types[ext]);
					} 
					else
					{
						this.trans_image_normal.Save(saveFileDialog1.FileName, this.image_types[ext]);
					}					
				}
			}
		
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
		       	switch(keyData)
		       	{
				case Keys.Escape:
					this.Close();
					Application.Exit();
					break;

			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		public static Bitmap ChangeOpacity(Image img, float opacityvalue)
		{
	    		Bitmap bmp = new Bitmap(img.Width,img.Height); // Determining Width and Height of Source Image
	    		Graphics graphics = Graphics.FromImage(bmp);
	    		System.Drawing.Imaging.ColorMatrix colormatrix = new System.Drawing.Imaging.ColorMatrix();
	    		colormatrix.Matrix33 = opacityvalue;
	    		System.Drawing.Imaging.ImageAttributes imgAttribute = new System.Drawing.Imaging.ImageAttributes();
	    		imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
	   		graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
	    		graphics.Dispose();   // Releasing all resource used by graphics 
	    		return bmp;
		}
		
		private static Image InvertImage(Image originalImg)
		{
		    	Bitmap invertedBmp = null;

		    	using (Bitmap originalBmp = new Bitmap(originalImg))
		    	{
				invertedBmp = new Bitmap(originalBmp.Width, originalBmp.Height);

				for (int x = 0; x < originalBmp.Width; x++)
				{
			    		for (int y = 0; y < originalBmp.Height; y++)
			    		{
						//Get the color
						Color clr = originalBmp.GetPixel(x, y);

						//Invert the clr
						clr = Color.FromArgb(255 - clr.R, 255 - clr.G, 255 - clr.B);

						//Update the color
						invertedBmp.SetPixel(x, y, clr);
			    		}
				}
		    	}

		    	return (Image)invertedBmp;
		}

		#region Windows Form Designer generated code
		
		private void InitializeComponent()
		{
			this.pb = new PictureBox();

			this.Controls.Add(this.pb);
			
			this.image_types.Add("png", System.Drawing.Imaging.ImageFormat.Png);
			this.image_types.Add("jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
			this.image_types.Add("bmp", System.Drawing.Imaging.ImageFormat.Bmp);

			this.KeyPress += new KeyPressEventHandler(ImagePreviewer_KeyPress);

		}
		
		#endregion
		
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			try
			{
				string[] args = Environment.GetCommandLineArgs();

				Application.Run(new ImagePreviewer(args));
			}
			catch
			{
			}
					
		}
        }
}