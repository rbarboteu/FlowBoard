using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
