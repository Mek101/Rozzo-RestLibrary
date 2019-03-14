using System;
using System.Text;
using Newtonsoft.Json;

namespace Rozzo_RestClient
{
    class ReadOnlyBook
    {
        [JsonProperty("ID")]
        public int ID { private set; get; }

        [JsonProperty("titolo")]
        public string Title { private set; get; }

        [JsonProperty("prezzo")]
        public int Price { private set; get; }

        [JsonProperty("sconto")]
        public float Discount { private set; get; }

        [JsonProperty("npagine")]
        public int Pages { private set; get; }

        [JsonProperty("annopubb")]
        public DateTime Publication { private set; get; }

        [JsonProperty("dataarch")]
        public DateTime Archiviation { private set; get; }

        [JsonProperty("reparto")]
        public string Repart { private set; get; }

        [JsonProperty("autore")]
        public string Author { private set; get; }


        public ReadOnlyBook(int id, string title, int price, int pages, DateTime publication, string repart, string author)
        {
            ID = id;
            Title = title;
            Price = price;
            Pages = pages;
            Publication = publication;
            Repart = repart;
            Author = author;
        }

        public override string ToString() { return Title + " " + Repart + " " + Price + " " + Discount + " " + Author; }
    }


    class BookWithUser
    {
        public BookWithUser(string user, ReadOnlyBook[] books)
        {
            User = user;
            Books = books;
        }

        [JsonProperty("user")]
        public string User { private set; get; }

        [JsonProperty("books")]
        public ReadOnlyBook[] Books { private set; get; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(User);
            foreach (ReadOnlyBook book in Books)
                builder.AppendLine(book.ToString());

            return builder.ToString();
        }
    }
}
