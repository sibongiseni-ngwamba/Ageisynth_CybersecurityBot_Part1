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

        // Predefined special questions and responses
        private string[] specialQuestions = {
            "how are you",
            "what is your purpose",
            "what can i ask about"
        };

        private string[] specialResponses = {
            "I am a bot.\nI'm here to help you.",
            "\nMy purpose is to provide useful cybersecurity advice.",
            "\nYou can ask me about the following topics: password, phishing, SQL injection, Attacks, malware, and firewalls."
        };
        public ageisynthBot_interface()
        {
        }// End of Constructor
    }// End of AgeisynthBot
}//End of Namespace