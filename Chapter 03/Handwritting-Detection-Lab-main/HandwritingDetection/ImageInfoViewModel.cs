using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandwritingDetection
{
    public class Word
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Line
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public List<Word> words { get; set; }
    }

    public class RecognitionResult
    {
        public List<Line> readResults { get; set; }
    }

    public class ImageInfoViewModel
    {
        public string status { get; set; }
        public RecognitionResult analyzeResult { get; set; }
    }
}