using System;
using System.IO;
using System.Drawing;
using Tesseract;
using SautinSoft;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocInfo
{
    class DocumentServise
    {
        public string DirectoryArchive { get; private set; }
        public string DirectoryResult { get; set; }
        private string DirectoryTemporary { get; set; }

        /// <summary>
        /// Логика работы с архивом документов
        /// </summary>
        /// <param name="directoryArchive">архив с документами</param>
        /// <param name="directoryResult">архив с результатом выполнения программы</param>
        public DocumentServise(string directoryArchive, bool createResult) 
        {
            if (string.IsNullOrEmpty(directoryArchive))
                throw new ArgumentNullException("Путь к каталогу архива документов отсутствует!");
            if (!Directory.Exists(directoryArchive))
                throw new Exception($"Калог к архиву по выбранному пути <{directoryArchive}> отсутствует!");

            this.DirectoryArchive = directoryArchive;
            this.DirectoryTemporary = @"..\..\DirectoryTemporary\";
            Directory.CreateDirectory(this.DirectoryTemporary);
            if (createResult)
            {
                this.DirectoryResult = @"..\..\Вывод\";
                Directory.CreateDirectory(this.DirectoryResult);
            }    
        }

        public List<Document> GetDocuments(IDocument document)
        {
            List<Document> documents = new List<Document>();
            string[] allPathFiles = Directory.GetFiles(this.DirectoryArchive, "*.*", SearchOption.AllDirectories);
            foreach (string path in allPathFiles)
            {
                documents.Add(document.AddDocument(path));
            }
            return documents; 
        }
        public void Start(List<Document> documents)
        {
            foreach (Document doc in documents)
            {
                doc.SearchInfoDoc(ReadDoc(doc));
                Console.WriteLine(doc.ToString());
            }
                
        }
        private string ReadDoc(Document document)
        {
            Console.WriteLine($"Текущий обрабатываемый документ: {System.IO.Path.GetFileName(document.Path)}");
            string result = string.Empty;
            if (document.Extension == ".pdf")
                result = OpenPDF(document.Path);

            return result;
        }

        private string OpenPDF(string path) //метод конвертирования PDF
        {
            //Создания экземпляра класса для конвертации pdf в jpg
            string txtFile = DirectoryTemporary + "textOCR.txt";
            PdfFocus f = new PdfFocus();
            f.OpenPdf(path);
            Console.WriteLine($"Колличество страниц: {f.PageCount}");
            if (f.PageCount > 0)
            {
                f.ImageOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                f.ImageOptions.Dpi = 300;

                //Деление каждой страницы pdf на изображения
                f.ToImage(DirectoryTemporary, "page");
            }
            using (FileStream fileStream = File.Open(txtFile, FileMode.Create))
            {
                using (StreamWriter strWrite = new StreamWriter(fileStream))
                {
                    for (int i = 1; i < f.PageCount + 1; i++)
                    {
                        string pathPage = DirectoryTemporary + $"page{i}.jpg";
                        try
                        {
                            using (TesseractEngine engine = new TesseractEngine(@"tessdata", "rus", EngineMode.Default))
                            {
                                using (var img = Pix.LoadFromFile(pathPage))
                                {
                                    using (var page = engine.Process(img))
                                    {
                                        Console.WriteLine("Качество скана: {0}", page.GetMeanConfidence());
                                        var text = page.GetText();
                                        strWrite.WriteLine($"{text}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);

                        }
                    }
                }
               
            }
            return txtFile;
        }
    }
}
