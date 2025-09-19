using AutoMapper;
using Retail.CheckList.Infrastructure.IMapping;

namespace Retail.CheckList.Infrastructure.Mapping.AM
{
    public class AMMapper : IAMapper
    {
        IMapper _mapper;
        public AMMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
        public TDest Map<TSrc, TDest>(TSrc src, TDest dest)
        {
            return _mapper.Map(src, dest);
        }

        public TDest Map<TDest>(object src)
        {
            return _mapper.Map<TDest>(src);
        }
    }
}
