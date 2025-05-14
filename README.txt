=====================================================
AgeisynthBot Cybersecurity Assistant - README FILE
=====================================================

Project Details
----------------
- Project Name: Ageisynth_CybersecurityBot_Part1
- Type: Console Application (.NET Framework)
- Purpose: Interactive cybersecurity education and awareness

==================
User Guide
==================

Getting Started
----------------
Running the Application
1. Launch the application by running the executable
2. An audio greeting will play (using `greeting.wav`)
   - The system automatically looks for this file in the root project directory
   - If the file is missing, an error message will be displayed
3. An ASCII art logo will appear (generated from `logo.jpg`)
   - The image is converted to ASCII art for display in the console
   - If the image file is missing, an error message will be displayed
4. The welcome banner will appear at the top of the console

Initial Setup
----------------
1. If this is your first time using the application, you'll be asked to enter your name
2. If you've used the application before, it will remember your name and ask if you're the same person
3. Once your identity is confirmed, the chatbot will welcome you and the conversation begins

==============================
Interacting with AgeisynthBot
==============================

Supported Cybersecurity Topics
-------------------------------
You can ask about various cybersecurity topics including:
- **Passwords** - Security practices, management, and two-factor authentication
- **Phishing** - Identifying and avoiding phishing attempts
- **Privacy** - Managing your online privacy settings and data
- **Malware** - Prevention, detection, and protection
- **Firewalls** - Configuration and best practices
- **Scams** - How to identify and avoid online scams
- **Attacks** - Types of cyber attacks and defenses
- **SQL Injection** - Understanding and preventing SQL-based attacks

Special Commands and Questions
--------------------------------
AgeisynthBot recognizes special phrases like:
- "how are you"
- "what is your purpose"
- "what can i ask about"
- "who made you"
- "help"
- "how do i exit"

Example Interactions
---------------------

```
You:-> Tell me about password security
AgeisynthBot:-> Make sure to use strong, unique passwords for each account. Aim for at least 12 characters with a mix of letters, numbers, and symbols.

You:-> I'm interested in phishing
AgeisynthBot:-> Great! I'll remember that you're interested in phishing. It's an important aspect of cybersecurity!
Here's something about phishing: Be cautious of emails asking for personal information. Legitimate organizations rarely request sensitive details via email.
```
==================
Advanced Features
==================

Memory and Personalization
---------------------------
- AgeisynthBot remembers your name between sessions
- It can track your interests in specific cybersecurity topics
- You can ask "what do you know about me" or "what do you remember" to see stored information

Sentiment Recognition
----------------------
The bot can detect and respond to different emotional states:
- If you seem worried, it will provide reassurance
- If you're confused, it will simplify explanations
- If you're frustrated, it will offer more patient guidance
- If you're curious, it will encourage your interest
- If you're happy, it will match your enthusiasm

Follow-up Questions
--------------------
Based on your conversation, the bot may ask relevant follow-up questions to provide more specific information about topics you're discussing.

===========================
Exiting the Application
===========================

To exit AgeisynthBot, simply type:
```
exit
```

Technical Notes
- User memory is saved to a file called `memory.txt` in the application directory
- The application uses a keyword-matching system to provide relevant responses
- Sentiment detection helps tailor responses to your current emotional state
- Audio greeting uses a `.wav` file that should be placed in the project root directory
- ASCII art logo is generated from a `logo.jpg` image file in the project root directory
- Make sure both media files are in the root directory of the project (NOT in `bin\Debug`)

---
========================================
Thank you for using AgeisynthBot!
Stay Cyber-Safe! 🛡️
========================================

========================================================================================
VERY IMPORTMANT added part2poe branch and merged it into the main branch on GITHUB!!!!!
========================================================================================
Links to updated GitHub: https://github.com/sibongiseni-ngwamba/Ageisynth_CybersecurityBot_Part1.git
