using AutoMapper;
using Rokys.Audit.Infrastructure.IMapping;

namespace Rokys.Audit.Infrastructure.Mapping.AM
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
