using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationApp
{
    public class Book : ItemBase
    {
        private readonly int itemID;
        private string title;
        private string genre;
        private string author;

        public override int ItemID => itemID;
        public override string Title
        {
            get => title;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    title = value;
                else
                    throw new ArgumentException("Başlık boş olamaz.");
            }
        }

        public override string Type { get; set; } = "Book";

        public override string Genre
        {
            get => genre;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    genre = value;
                else
                    throw new ArgumentException("Tür boş olamaz.");
            }
        }

        public string Author
        {
            get => author;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    author = value;
                else
                    throw new ArgumentException("Yazar adı boş olamaz.");
            }
        }

        public Book(string title, string genre, string author)
        {
            Title = title;
            Genre = genre;
            Author = author;
        }

        public override string GetInfo()
        {
            return $"{Title} (Book - {Genre}) by {Author}";
        }
    }

}
