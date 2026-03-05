using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public Project Project { get; set; } = null!;
}