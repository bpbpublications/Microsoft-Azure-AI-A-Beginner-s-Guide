using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HandwritingDetection
{
    internal class Program

    {
        private static string subscriptionKey = "Subscription key"; //Replace subscription key

        private static string
            endpoint = "https://retailocr.cognitiveservices.azure.com/vision/v1.0/ocr?"; //Replace endpoint URL

        private static void Main()

        {
            var ch = "1";

            Console.WriteLine("HandWritten Text Extraction:");

            while (ch == "1")

            {
                Console.Write("Enter the path to an image with text you wish to read: ");

                string imageFilePath = Console.ReadLine();

                if (File.Exists(imageFilePath))
                {
                    Console.WriteLine("\nWait a moment for the results to appear.\n");
                    MakeRequest(imageFilePath).Wait();
                }
                else

                {
                    Console.WriteLine("\nInvalid file path");
                }

                Console.WriteLine("\nPress Enter 0 to exit and 1 to continue.......");

                ch = Console.ReadLine();
            }
        }

        private static async Task<string> MakeRequest(string imageFilePath)

        {
            string extractedResult = "";

            string operationLocation;

            string result;

            int i = 0;

            var client = new HttpClient();

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            HttpResponseMessage response;

            // Request headers

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters

            queryString["language"] = "en";

            var uri = endpoint + queryString;

            // Request body

            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new
                       ByteArrayContent(byteData))

            {
                content.Headers.ContentType = new
                    MediaTypeHeaderValue("application/octet-stream");

                // The first REST call starts the async process to analyze the written text in the image.

                response = await client.PostAsync(uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                operationLocation =
                    response.Headers.GetValues(
                        "Operation-Location").FirstOrDefault();
            }
            else

            {
                // Display the JSON error data.

                string errorString = await
                    response.Content.ReadAsStringAsync();

                Console.WriteLine("\n\nResponse:\n{0}\n",
                    JToken.Parse(errorString).ToString());

                Console.WriteLine(errorString);

                return errorString;
            }

            // Note: The response may not be immediately available.

            // Recognition is an async operation that can take a variable Amount of time.

            do
            {
                System.Threading.Thread.Sleep(1000);

                response = await client.GetAsync(operationLocation);

                result = await response.Content.ReadAsStringAsync();

                ++i;
            } while (i < 11 && result.IndexOf("\"status\":\"Succeeded\"") == -1);

            //If it is success it will execute further process.

            if (response.IsSuccessStatusCode)

            {
                // The JSON response mapped into respective view model.

                var responeData =
                    JsonConvert.DeserializeObject<ImageInfoViewModel>(result);

                var linesCount =
                    responeData.analyzeResult.readResults.Count;

                for (int j = 0; j < linesCount; j++)

                {
                    var imageText = responeData.analyzeResult.readResults[j].text;

                    extractedResult += imageText + Environment.NewLine;
                }
            }

            Console.WriteLine("Result:\n------------------------\n"
                              + extractedResult + "---------------------------\n");

            return extractedResult;
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)

        {
            // Open a read-only file stream for the specified file.

            using (FileStream fileStream =
                   new FileStream(imageFilePath, FileMode.Open,
                       FileAccess.Read))

            {
                // Read the file's contents into a byte array.

                BinaryReader binaryReader = new BinaryReader(fileStream);

                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}