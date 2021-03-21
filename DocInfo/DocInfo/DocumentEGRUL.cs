using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DocInfo
{
    class DocumentEGRUL : Document
    {
        #region Свойства
        /// <summary>
        /// Полное наименование организации
        /// </summary>
        public string NameFull { get; set; }
        /// <summary>
        /// Сокращенное наименование организации
        /// </summary>
        public string NameShort { get; set; }
        /// <summary>
        /// Дата постановки на учет в налоговом органе
        /// </summary>
        public DateTime DateReg { get; set; }
        /// <summary>
        /// Уставной капитал
        /// </summary>
        public decimal Capital { get; set; }
        #endregion

        public DocumentEGRUL() { }

        /// <summary>
        /// Выписки из ЕГРЮЛ
        /// </summary>
        /// <param name="patch">путь к файлу документа</param>
        public DocumentEGRUL(string path) : base(path) 
        {
            this.NameFull = string.Empty;
            this.NameShort = string.Empty;
            this.Capital = 0;

        }

        /// <summary>
        /// Создания объекта класса Document
        /// </summary>
        /// <param name="path">путь к файлу документа</param>
        /// <returns></returns>
        public override Document AddDocument(string path)
        {
            return new DocumentEGRUL(path);
        }

        public override void SearchInfoDoc(string txtFile)
        {
            string[] linesText = File.ReadAllLines(txtFile);
            this.SearchNameFull(linesText);
            this.SearchNameShort(linesText);
            this.SearchDateReg(linesText);
            this.SearchCapital(linesText);
            
        }
        private void SearchNameFull(string[] linesText)
        {
            for (int i = 0; i < linesText.Length; i++)
                if (Regex.IsMatch(linesText[i], "Полное наименование на русском языке", RegexOptions.IgnoreCase))
                {
                    this.NameFull = linesText[i] + linesText[i + 1];
                    this.NameFull = this.NameFull.Replace("1 — |Полное наименование на русском языке ", string.Empty);
                    return;
                }
        }
        private void SearchNameShort(string[] linesText)
        {
            for (int i = 0; i < linesText.Length; i++)
                if (Regex.IsMatch(linesText[i], "Сокращенное наименование на русском", RegexOptions.IgnoreCase))
                {
                    this.NameShort = linesText[i];
                    this.NameShort = this.NameShort.Replace("3 — |Сокращенное наименование на русском ", string.Empty);
                    return;
                }
        }
        private void SearchDateReg(string[] linesText)
        {
            for (int i = 0; i < linesText.Length; i++)
                if (Regex.IsMatch(linesText[i], "14 |Дата регистрации ", RegexOptions.IgnoreCase))
                {
                    string result = linesText[i].Replace("14 |Дата регистрации ", string.Empty);
                    this.DateReg = DateTime.ParseExact(result, "d", null);
                    return;
                }
        }
        private void SearchCapital(string[] linesText)
        {
            for (int i = 0; i < linesText.Length; i++)
                if (Regex.IsMatch(linesText[i], @"Размер \(в рублях\)", RegexOptions.IgnoreCase))
                {

                    string result = linesText[i].Replace("28 |Размер (в рублях) ", string.Empty);
                    decimal res = 0;
                    decimal.TryParse(result, out res);
                    this.Capital = res;
                    return;
                }
        }
        public override string ToString()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.AppendLine($"Полное наименование организации: {this.NameFull}");
            strBuild.AppendLine($"Сокращенное наименование организации: {this.NameShort}");
            strBuild.AppendLine($"Дата постановки на учет в налоговом органе: {this.DateReg.ToString("d")}");
            strBuild.AppendLine($"Уставной капитал: {this.Capital}");

            return strBuild.ToString();
        }
    }
}
