using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBoard.Application.DTOs.Tasks;

public class CreateTaskRequest
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
}