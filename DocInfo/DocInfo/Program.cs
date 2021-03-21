using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            DocumentServise servise = new DocumentServise(@"..\..\Архив\", true);
            List<Document> documents = servise.GetDocuments(new DocumentEGRUL());
            servise.Start(documents);
            Console.ReadKey();
        }
    }
}
