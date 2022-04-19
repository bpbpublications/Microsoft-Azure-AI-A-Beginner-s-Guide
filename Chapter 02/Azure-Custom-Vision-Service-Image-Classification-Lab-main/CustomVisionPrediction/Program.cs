using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CustomVisionPrediction
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Enter image file path
            Console.Write("Enter image file path: ");
            string filePath = Console.ReadLine();
            PredictImageOnRequest(filePath).Wait();
            Console.WriteLine("\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        public static async Task PredictImageOnRequest(string _filePath)
        {
            // Add valid Prediction Key 
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "Replace your Prediction-Key");

            // Replace the below string with the Prediction Url 
            string url = "Replace your prediction ";
            HttpResponseMessage responseMessage;
            byte[] byteData = ConvertImageToByteArray(_filePath);
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                responseMessage = await client.PostAsync(url, content);
                Console.WriteLine(await responseMessage.Content.ReadAsStringAsync());
            }
        }

        private static byte[] ConvertImageToByteArray(string filePath)
        {
            FileStream fileStreamReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binReader = new BinaryReader(fileStreamReader);
            return binReader.ReadBytes((int) fileStreamReader.Length);
        }
    }
}