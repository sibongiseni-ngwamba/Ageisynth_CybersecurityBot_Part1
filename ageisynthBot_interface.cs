using System;
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

        // Constructor - This runs automatically when a new object of ChatbotResponse is created
        public ageisynthBot_interface()
        {
        }// End of Constructor

        // Method to check if user's question matches a predefined special question
        private string HandleSpecialQuestions(string userInput)
        {
            for (int i = 0; i < specialQuestions.Length; i++)
            {
                if (userInput.Contains(specialQuestions[i]))
                {
                    return specialResponses[i]; // Return matching special response
                }
            }
            return null;
        }

        // Filters user's input and matches keywords with known responses
        private string ApplyFilter(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn’t quite understand that. Could you rephrase?";
            }

            string[] store_word = userInput.ToLower().Split(' '); // Break input into lowercase words
            ArrayList store_final_words = new ArrayList();

            // Remove ignored words from the input
            foreach (string word in store_word)
            {
                if (!ignore.Contains(word))
                {
                    store_final_words.Add(word);
                }
            }

            string message = string.Empty;
            bool found = false;

            // Compare remaining words to stored replies
            foreach (string keyword in store_final_words)
            {
                foreach (string reply in replies)
                {
                    if (reply.ToLower().IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        message += $"- {reply} \n";
                        found = true;
                    }
                }
            }

            // Return matched message or fallback

            //if satement display anwser or error message
            if (found)
            {
                return message; // If matches were found, return the constructed message 
            }
            else
            {
                return "I didn’t quite understand that. Could you rephrase?"; // If no matches were found, return a default response
            }
        }

        // Stores list of valid chatbot replies for different topics
        private void store_replies()
        {
            replies.Add("Password need to be protected and kept safe.");
            replies.Add("Use strong, unique passwords and enable two-factor authentication.");
            replies.Add("Never reuse passwords across different sites.");
            replies.Add("Consider using a password manager for storing your passwords.");
            replies.Add("SQL injection is a serious attack method where attackers exploit vulnerabilities in the application.");
            replies.Add("To prevent SQL injection, always use parameterized queries.");
            replies.Add("Never directly insert user input into SQL queries.");
            replies.Add("Ensure your application is updated regularly to patch any SQL injection vulnerabilities.");
            replies.Add("Phishing attacks trick users into revealing sensitive information like passwords.");
            replies.Add("Always verify the sender’s email address before clicking on any links in emails.");
            replies.Add("Do not open email attachments from unknown senders.");
            replies.Add("Look for signs of phishing, such as suspicious email addresses and incorrect grammar.");
            replies.Add("Attacking in cybersecurity involves exploiting vulnerabilities to compromise systems.");
            replies.Add("Some common attack techniques include malware injection, social engineering, and DDoS attacks.");
            replies.Add("Preventing attacks requires using firewalls, intrusion detection systems, and constant monitoring.");
            replies.Add("Ensure software is regularly updated to patch security vulnerabilities.");
            replies.Add("Malware is software specifically designed to disrupt, damage, or gain unauthorized access to a system.");
            replies.Add("Malware can be spread through phishing emails, infected websites, or malicious downloads.");
            replies.Add("To prevent malware infections, use up-to-date antivirus software and avoid suspicious downloads.");
            replies.Add("Keep your operating system and applications updated to protect against known malware threats.");
            replies.Add("A firewall is a network security system that monitors and controls incoming and outgoing traffic.");
            replies.Add("Firewalls can be either hardware or software-based and help protect against unauthorized access.");
            replies.Add("Configuring firewalls properly is essential to block malicious traffic and reduce attack vectors.");
            replies.Add("Ensure your firewall rules are configured correctly and regularly updated to reflect the current threat landscape.");
        }

        // Stores words that are not useful for identifying keywords (filler or common words)
        private void store_ignore()
        {
            // Common words that don't add meaning to a question
            string[] ignoreWords = {
                "tell", "and", "me", "about", "ensure", "for", "how", "what", "is", "you", "your", "the", "can",
                "do", "would", "will", "should", "could", "are", "am", "was", "were", "be", "been", "being", "it",
                "that", "this", "these", "those", "here", "there", "where", "when", "why", "who", "which", "um",
                "uh", "no", "like", "know", "so", "actually", "okay", "sure", "yeah", "yep", "nope", "nah", "not",
                "very", "quite", "slightly", "a", "an", "in", "on", "at", "by", "of", "to", "with", "my", "i",
                "we", "they", "us", "it’s", "that’s", "don’t", "won’t", "can’t", "doesn’t", " ", "didn’t", ",",
                ".", "?", "!", ";", ":"
            };

            // Add all ignore words to the ArrayList
            foreach (string word in ignoreWords)
            {
                ignore.Add(word);
            }
        }

        // Creates a typing animation effect for bot responses
        private void TypeEffect(string message)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(50); // Typing speed
            }
            Console.WriteLine(); // Move to the next line after typing
        }// End of Type Effect
    }// End of AgeisynthBot
}//End of Namespace