using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureAI.ImageProcessing_AnalyzeImage
{
    internal class Program
    {
        // Replace computerVisionsubscriptionKey with your subscription key
        private const string computerVisionsubscriptionKey = "Your SubscriptionKey";


        // Replace computerVisionsubscriptionKey with the correct Azureregion for the API
        private const string baseURL = "https://centralindia.api.cognitive.microsoft.com//vision/v2.0/analyze";

        private static void Main(string[] args)
        {
            //Prompt the user for an input image file 
            Console.Write("Enter the path of the image: ");
            string filePath = Console.ReadLine();
            if (File.Exists(filePath))
            {
                Console.WriteLine("\n Results are getting processed.....\n");
                ProcessImage(filePath).Wait();
            }
            else
            {
                Console.WriteLine("The given file path is invalid");
            }

            Console.WriteLine("\nPress Enter to exit the program...");
            Console.ReadLine();
        }

        /// <summary> 
        /// /// The below function analyses the image 
        /// /// <param name="filePath">The name of the file to be analyzed</param> 
        /// 
        private static async Task ProcessImage(string filePath)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", computerVisionsubscriptionKey);
                string requestParameters = "visualFeatures=Categories,Description,Color";
                string uri = baseURL + "?" + requestParameters;
                HttpResponseMessage response;
                byte[] byteData = GetPictureAsByteArray(filePath);
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\nResponse:\n\n{0}\n", JToken.Parse(jsonResponse).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        private static byte[] GetPictureAsByteArray(string filePath)
        {
            using (FileStream fileStreamReader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array. 
                BinaryReader binReader = new BinaryReader(fileStreamReader);
                return binReader.ReadBytes((int)fileStreamReader.Length);
            }
        }
    }
}