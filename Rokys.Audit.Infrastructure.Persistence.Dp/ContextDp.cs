using Dapper.FluentMap;
using Rokys.Audit.Infrastructure.Persistence.Dp.Mapping;

namespace Rokys.Audit.Infrastructure.Persistence.Dp
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
