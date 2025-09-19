using Retail.CheckList.DTOs.Responses.Common;

namespace Retail.CheckList.Services.Interfaces
{
    public interface IBaseService<TRequest, TResponse>
    {
        public Task<ResponseDto<IEnumerable<TResponse>>> Get(object filter);
        public Task<ResponseDto<TResponse>> GetById(object id);
        public Task<ResponseDto<TResponse>> Update(object id, TRequest requestDto);
        public Task<ResponseDto<TResponse>> Create(TRequest requestDto);
        public Task<ResponseDto> Delete(object id);
    }
}
