using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BuildingBlocks.CQRS
{
    //Note: Unit is null in MediatR
    public interface ICommandHander<in TCommand> : IRequestHandler<TCommand, Unit>
        where TCommand: ICommand<Unit>
    {

    }
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand: ICommand<TResponse>
        where TResponse: notnull
    {

    }

}
