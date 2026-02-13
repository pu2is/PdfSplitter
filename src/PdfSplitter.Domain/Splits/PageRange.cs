namespace PdfSplitter.Domain.Splits;

public sealed record PageRange(int StartPage, int EndPage)
{
    public bool IsValidFor(int totalPages)
    {
        return StartPage >= 1 &&
               EndPage >= StartPage &&
               EndPage <= totalPages;
    }
}
