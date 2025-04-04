using System.IO;
using System;
using System.Drawing;

namespace Ageisynth_CybersecurityBot_Part1
{
    public class ascii_logo
    {

        // Constructor: When this class is created, it calls the logo_creator
        public ascii_logo()
        {
        }
        // Method to create and display the ASCII art logo from an image
        private void logo_creator()
        {
            // Get the full directory path where the program is running
            string full_location = AppDomain.CurrentDomain.BaseDirectory;

            // Clean up the path by removing the 'bin\Debug\' part so we can get back to the main project folder
            string new_location = full_location.Replace("bin\\Debug\\", "");

            // Combine the cleaned path with the image file name to get the full image path
            string full_path = Path.Combine(new_location, "logo.jpg");

            // Create a Bitmap object using the image at the specified path
            Bitmap image = new Bitmap(full_path);

            // Resize the image to 150 pixels wide and 120 pixels tall to fit better in the console
            image = new Bitmap(image, new Size(150, 120));

            // Loop through every row (height) of pixels in the image
            for (int height = 0; height < image.Height; height++)
            {
                // Loop through every column (width) of pixels in the image
                for (int width = 0; width < image.Width; width++)
                {
                    // Get the color of the current pixel
                    Color pixelColor = image.GetPixel(width, height);

                    // Convert the pixel color to a grayscale value (average of R, G, and B)
                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // Choose a character based on how light or dark the pixel is
                    // Lighter pixels get lighter characters, darker pixels get heavier characters
                    char asciichar = gray > 200 ? '-' :
                                     gray > 150 ? '*' :
                                     gray > 100 ? 'o' :
                                     gray > 50 ? '#' : '@';

                    // Print the character without moving to a new line
                    Console.Write(asciichar);
                }

                // After finishing a row, move to the next line
                Console.WriteLine();
            } // End of nested loop
        } // End of logo_creator method
    }
}