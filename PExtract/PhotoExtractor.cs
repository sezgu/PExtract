using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PExtract
{
    class PhotoExtractor
    {
        public struct ExtractionInfo
        {
            public FileInfo Source { get; set; }
            public FileInfo Destination { get; set; }
            public Exception Error { get; set; }
        }

        private DirectoryInfo m_Source;
        private DirectoryInfo m_Destination;
        private string[] m_Extensions;
        private IEnumerable<ExtractionInfo> m_Extraction;
        private Dictionary<string, FileInfo> m_ExtractedFiles = new Dictionary<string, FileInfo>();

        public PhotoExtractor(DirectoryInfo source, string destination, params string[] extensions)
        {
            if (extensions == null || extensions.Length == 0)
                extensions = new string[] { ".jpg", ".jpeg", ".png" };
            m_Extensions = extensions;
            m_Source = source;
            m_Destination = Directory.CreateDirectory(destination);
        }

        public IEnumerable<ExtractionInfo> Extraction
        {
            get
            {
                if (m_Extraction != null)
                    return m_Extraction;
                return m_Extraction = GetExtraction(m_Source);
            }
        }

        private IEnumerable<ExtractionInfo> GetExtraction(DirectoryInfo directory)
        {
            var files = m_Extensions.Select(e => directory.EnumerateFiles("*" + e)).SelectMany(f => f);

            foreach(var file in files)
            {
                int count = 1;
                Func<string> destinationFileName = () => 
                {
                    return file.Name.Substring(0, file.Name.Length - file.Extension.Length) + (count == 1 ? "" : "_" + count.ToString()) + file.Extension;
                };
                while (m_ExtractedFiles.ContainsKey(destinationFileName()))
                    count++;

                FileInfo destFile = null;
                Exception exception = null;
                try
                {
                    destFile = file.CopyTo(Path.Combine(m_Destination.FullName, destinationFileName()));
                    m_ExtractedFiles.Add(destFile.Name, destFile);
                }
                catch(Exception ex)
                {
                    exception = ex;
                }
                yield return new ExtractionInfo() { Source = file, Destination = destFile , Error = exception};
            }

            foreach(var extractions in directory.EnumerateDirectories()
                .Where(d => !d.Name.StartsWith("$") && !(d.Name == "System Volume Information"))
                .Where(d => !d.Name.StartsWith("$"))
                .Select(d => GetExtraction(d)))
            {
                foreach (var ex in extractions)
                    yield return ex;
            }

            yield break;
        }

    }
}
