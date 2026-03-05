using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBoard.Application.DTOs.Tasks;

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}