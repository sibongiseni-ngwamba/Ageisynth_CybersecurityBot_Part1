using System.IO;
using System.Media;
using System;

namespace Ageisynth_CybersecurityBot_Part1
{
    // Class to handle playing a voice greeting when an object is created
    public class voice_greeting
    {
        // Constructor: Automatically plays the welcome message when an object is created
        public voice_greeting()
        {
        }
        // Private method to play the welcome audio message
        private void playWelcome_message()
        {
            // Get the full location of the application's base directory (e.g., bin\Debug\)
            string full_location = AppDomain.CurrentDomain.BaseDirectory;

            // Remove "bin\Debug\" from the path to locate the root of the project directory
            string new_path = full_location.Replace("bin\\Debug\\", "");

            // Try to play the audio file and catch any errors if it fails
            try
            {
                // Combine the new base path with the audio file name to create the full path
                string full_path = Path.Combine(new_path, "greeting.wav");

                // Create an instance of SoundPlayer to load and play the audio
                using (SoundPlayer player = new SoundPlayer(full_path))
                {
                    // Play the audio file asynchronously (starts playing)
                    player.Play();

                    // Play the audio file synchronously (waits for it to finish)
                    player.PlaySync();
                } // SoundPlayer is disposed here after use
            }
            catch (Exception audioErrror)
            {
                // Display any errors that occur while trying to play the audio
                Console.WriteLine(audioErrror.Message);
            }
        }
    }
}