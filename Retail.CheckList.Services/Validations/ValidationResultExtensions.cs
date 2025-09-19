using FluentValidation.Results;
using Rokys.Audit.DTOs.Responses.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Validations
{
    public static class ValidationResultExtensions
    {
        public static ResponseDto ToResponse<T>(this T validation) where T : ValidationResult
        {
            var response = ResponseDto.Create();
            if(!validation.IsValid)
                foreach (var item in validation.Errors)
                {
                    response.Messages.Add(new ApplicationMessage { Message = item.ErrorMessage, MessageType = ApplicationMessageType.Error });
                }       
              
            return response;
        }
    }
}
