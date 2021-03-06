﻿using System;
using System.Threading.Tasks;
using DFramework.Utilities;

namespace DFramework
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public class DefaultCommandBus : ICommandBus
    {
        private ICommandExecutorContainer _executorContainer;

        public DefaultCommandBus(ICommandExecutorContainer executorContainer)
        {
            Check.Argument.IsNotNull(executorContainer, "executorContainer");

            this._executorContainer = executorContainer;
        }

        public virtual void Send<TCommand>(TCommand cmd) where TCommand : ICommand
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            this.SendAsync(cmd).Wait(100);

        }
        public virtual Task SendAsync<TCommand>(TCommand cmd) where TCommand : ICommand
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            try
            {
                var executor = this._executorContainer.FindExecutor<TCommand>();

                if (executor == null)
                    throw new UnknowExecption("Faile to find " + typeof(TCommand).Name + "'s executor.");

                return executor.ExecuteAsync(cmd);
            }
            catch (IoCException)
            {
                throw;
            }
            catch (CommandExecutionException ex)
            {
                throw ex;
            }
            catch (SystemException ex)
            {
                throw new UnknowExecption("Faile to execute " + typeof(TCommand).Name + ",see the inner exception for detail.", ex);
            }
            catch (Exception ex)
            {
                Log.Error("send command error", ex);
                throw ex;
            }
        }
    }
}
