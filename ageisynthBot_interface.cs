using System.Collections;

namespace Ageisynth_CybersecurityBot_Part1
{
    // Class responsible for chatbot interaction and response handling
    public class ageisynthBot_interface
    {
        // Global variables to store user name and input
        private string user_name = string.Empty;
        private string user_asking = string.Empty;

        // ArrayLists to store possible replies and words to ignore in user input
        private ArrayList replies = new ArrayList();
        private ArrayList ignore = new ArrayList();

        public ageisynthBot_interface()
        {
        }// End of Constructor
    }// End of AgeisynthBot
}//End of Namespace