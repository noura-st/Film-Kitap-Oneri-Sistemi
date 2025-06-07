using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationApp
{
    public class Item : ItemBase
    {
        private readonly int itemID;
        private string title;
        private string type;
        private string genre;

        public override int ItemID => itemID;

        public override string Title
        {
            get => title;
            set => title = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Başlık boş olamaz.");
        }

        public override string Type
        {
            get => type;
            set => type = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Tür boş olamaz.");
        }

        public override string Genre
        {
            get => genre;
            set => genre = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Tür boş olamaz.");
        }

        public Item(int itemId, string title, string type, string genre)
        {
            this.itemID = itemId;
            this.Title = title;
            this.Type = type;
            this.Genre = genre;
        }

        public override string GetInfo()
        {
            return $"{Title} ({Type} - {Genre})";
        }

        public override string ToString()
        {
           return $"{Title} ({Type} - {Genre})";
        }
    }
}
