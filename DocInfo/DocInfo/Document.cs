using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocInfo
{
    abstract class Document : IDocument
    {

        protected Status StatusDoc;
        public string Path { get; set; }
        public string Extension { get ; set; }

        public Document() { }

        public Document(string path)
        {
            this.StatusDoc = CheckFile(path);
            if (this.StatusDoc == Status.NotValue)
                path = string.Empty;
            this.Path = path;

        }

        /// <summary>
        /// Проверка файла с выставлением статуса
        /// </summary>
        /// <param name="path">путь к файлу документа</param>
        /// <returns>статус</returns>
        private Status CheckFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return Status.NotValue;
            if (!File.Exists(path))
                return Status.NotFound;

            this.Extension = System.IO.Path.GetExtension(path);
            if (Extension != ".pdf"
                || Extension != ".jpg"
                || Extension != ".jpeg")
                return Status.NotSupport;

            return Status.Verified;
        }
        public abstract Document AddDocument(string path);
        public abstract void SearchInfoDoc(string txtFile);
        public abstract override string ToString();

        /// <summary>
        /// Текущий статус документа
        /// </summary>
        protected enum Status
        {
            NotValue, //отсутствие значения
            NotFound, //файл не найден
            NotSupport, //файл не поддерживается
            Verified //файл проверен
        }

    }
}
