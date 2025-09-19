namespace Retail.CheckList.Infrastructure.IMapping
{
    public interface IAMapper
    {

        TDest Map<TSrc, TDest>(TSrc src, TDest dest);

        TDest Map<TDest>(object src);
    }
}
