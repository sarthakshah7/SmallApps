using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace MailApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // --- Configuration for your Gmail account ---
            // IMPORTANT: For security, DO NOT use your regular Gmail password here.
            // You must generate an 'App password' for your Google account if you have 2FA enabled.
            // Go to your Google Account -> Security -> App passwords.
            string senderEmail = "sender"; // Your Gmail address
            string senderAppPassword = "apppass"; // Your generated App Password
            string recipientEmail = "receiver"; // Recipient's email address
            string attachmentFileName = string.Empty;
            string sourceFilePath = string.Empty;

            // --- Create a dictionary to hold attachments (demonstrative) ---
            Dictionary<string, Attachment> attachmentsDictionary = new Dictionary<string, Attachment>();
            MemoryStream pdfStream = null; // Declare outside try block for finally access

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    attachmentFileName = $"racer-{(i + 1)}.jpg";
                    sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, attachmentFileName);

                    byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
                    pdfStream = new MemoryStream(fileBytes); // Create MemoryStream from file bytes

                    // Create an Attachment from the MemoryStream
                    // The "document.pdf" here is the filename that will appear in the email.
                    Attachment attachmentFromStream = new Attachment(pdfStream, attachmentFileName, "image/jpg");

                    // Add the attachment to our dictionary (demonstrates storing/managing attachments)
                    attachmentsDictionary.Add($"{(i + 1)} " + attachmentFileName, attachmentFromStream);

                    Console.WriteLine($"File '{$"{(i + 1)} " + attachmentFileName}' read into MemoryStream and added to dictionary.");
                }

                // --- Read the file into a MemoryStream ---
                for (int i = 0; i < 10; i++)
                {
                    attachmentFileName = $"document-{(i+1)}.pdf";
                    sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, attachmentFileName);

                    byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
                    pdfStream = new MemoryStream(fileBytes); // Create MemoryStream from file bytes

                    // Create an Attachment from the MemoryStream
                    // The "document.pdf" here is the filename that will appear in the email.
                    Attachment attachmentFromStream = new Attachment(pdfStream, attachmentFileName, "application/pdf");

                    // Add the attachment to our dictionary (demonstrates storing/managing attachments)
                    attachmentsDictionary.Add($"{(i + 1)} " + attachmentFileName, attachmentFromStream);

                    Console.WriteLine($"File '{$"{(i + 1)} " + attachmentFileName}' read into MemoryStream and added to dictionary.");
                }
            }
            catch (Exception streamEx)
            {
                Console.WriteLine($"Error processing attachment to memory stream: {streamEx.Message}");
                // Ensure stream is disposed even if an error occurs here
                pdfStream?.Dispose();
                pdfStream = null; // Set to null to indicate it's not valid for attachment
            }

            // --- Create the email message ---
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(recipientEmail);
            mail.Subject = "Test Email from C# Console App with PDF from Memory Stream";
            mail.Body = "Hello from your C# .NET Framework 4.8 Console Application! This email includes a PDF attachment created from a MemoryStream.";
            mail.IsBodyHtml = false; // Set to true if your body contains HTML

            // --- Add the PDF attachment from the dictionary to the email ---
            foreach (var obj in attachmentsDictionary)
            {
                mail.Attachments.Add(obj.Value);
                Console.WriteLine($"Attached '{obj.Key}' from dictionary to email.");
            }

            //if (attachmentsDictionary.ContainsKey(attachmentFileName))
            //{
            //    mail.Attachments.Add(attachmentsDictionary[attachmentFileName]);
            //    Console.WriteLine($"Attached '{attachmentFileName}' from dictionary to email.");
            //}
            //else if (pdfStream != null)
            //{
            //    // Fallback: If for some reason it wasn't added to dictionary but stream is valid,
            //    // add it directly. This case is less likely if the primary logic works.
            //    Console.WriteLine("Attempting to attach directly from memory stream (dictionary fallback).");
            //    mail.Attachments.Add(new Attachment(pdfStream, attachmentFileName, "application/pdf"));
            //}
            //else
            //{
            //    Console.WriteLine("No valid attachment found in dictionary or memory stream to add to email.");
            //}
            // --- Configure the SMTP client for Gmail ---
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587; // Gmail's SMTP port for TLS/STARTTLS
            smtpClient.EnableSsl = true; // Enable SSL (secure connection)
            smtpClient.UseDefaultCredentials = false; // Do not use default credentials
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderAppPassword);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; // Specify delivery method as network

            try
            {
                // --- Send the email ---
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (SmtpException ex)
            {
                // Catch specific SMTP errors
                Console.WriteLine($"SMTP Error: {ex.StatusCode}");
                Console.WriteLine($"Error Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine("Please ensure you have generated an 'App password' for your Gmail account and are using it correctly.");
                Console.WriteLine("Also, check your internet connection and firewall settings.");
            }
            catch (Exception ex)
            {
                // Catch any other general exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
