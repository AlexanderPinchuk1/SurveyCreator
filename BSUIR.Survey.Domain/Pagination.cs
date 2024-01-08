using System.ComponentModel.DataAnnotations;

namespace BSUIR.Survey.Domain
{
    public class Pagination
    {
        [Required]
        [Range(1, 100)]
        [Display(Name = "Items count per page")]
        public int ItemCountPerPage { get; set; }

        [Required]
        public int PageIndex { get; set; }

        public string? SearchKeyWord { get; set; }


        public Pagination(int itemCountPerPage = 5, int pageindex = 0, string? searchKeyWord = null)
        {
            ItemCountPerPage = itemCountPerPage;
            PageIndex = pageindex;
            SearchKeyWord = searchKeyWord;
        }


        public Pagination()
        {
            ItemCountPerPage = 5;
            PageIndex = 0;
            SearchKeyWord = null;
        }
    }
}
