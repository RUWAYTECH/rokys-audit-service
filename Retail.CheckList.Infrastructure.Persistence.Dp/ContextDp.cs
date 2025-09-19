using Dapper.FluentMap;
using Retail.CheckList.Infrastructure.Persistence.Dp.Mapping;

namespace Retail.CheckList.Infrastructure.Persistence.Dp
{
    public class ContextDp
    {
        public ContextDp()
        {

        }
        public static void Config()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new ActionMap());
            });
        }
    }
}
