namespace BSUIR.Survey.Foundation
{
    public class PaginationValidator
    {
        public static int ValidateNumberOfItemsPerPage(int numItemsPerPage)
        {
            numItemsPerPage = numItemsPerPage switch
            {
                < 1 => 1,
                > 100 => 100,
                _ => numItemsPerPage
            };

            return numItemsPerPage;
        }

        public static int ValidateNumberOfPages(int pageIndex, int itemCountPerPage, int totalCount)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            else if (pageIndex > Math.Ceiling((double)totalCount / itemCountPerPage) - 1)
            {
                pageIndex = (int)Math.Ceiling((double)totalCount / itemCountPerPage) - 1;
            }

            return pageIndex;
        }
    }
}
