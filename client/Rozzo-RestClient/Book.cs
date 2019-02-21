using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rozzo_RestClient
{
    class Book
    {
        public int ID { private set; get; }
        public string Title { private set; get; }
        public int Price { private set; get; }
        public int Pages { private set; get; }
        public DateTime Publication { private set; get; }
        public string Repart { private set; get; }
        public string Author { private set; get; }

        public Book(int id, string title, int price, int pages, DateTime publication, string repart, string author)
        {
            ID = id;
            Title = title;
            Price = price;
            Pages = pages;
            Publication = publication;
            Repart = repart;
            Author = author;
        }
    }
}
