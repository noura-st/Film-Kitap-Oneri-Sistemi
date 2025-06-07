using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationApp
{
    public class Film : ItemBase
    {
        private readonly int itemID;
        private string title;
        private string genre;
        private string director; 

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

        public override string Type { get; set; } = "Film";

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

        public string Director
        {
            get => director;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    director = value;
                else
                    throw new ArgumentException("Yönetmen adı boş olamaz.");
            }
        }

        public Film(string title, string genre, string director)
        {
            Title = title;
            Genre = genre;
            Director = director;
        }

        public override string GetInfo()
        {
            return $"{Title} (Film - {Genre}), directed by {Director}";
        }

        public override string ToString()
        {
            return GetInfo();
        }
    }
}
