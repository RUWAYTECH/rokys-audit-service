using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IBaseService<TRequest, TResponse>
    {
        [Obsolete("This method is not implemented.")]
        public Task<ResponseDto<IEnumerable<TResponse>>> Get(object filter) => Task.FromResult(new ResponseDto<IEnumerable<TResponse>>());
        public Task<ResponseDto<TResponse>> GetById(Guid id);
        public Task<ResponseDto<TResponse>> Update(Guid id, TRequest requestDto);
        public Task<ResponseDto<TResponse>> Create(TRequest requestDto);
        public Task<ResponseDto> Delete(Guid id);
    }
}
