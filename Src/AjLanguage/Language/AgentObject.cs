namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Text;

    public class AgentObject : DynamicClassicObject
    {
        // TODO 100 is hard coded
        private QueueChannel channel = new QueueChannel(100);

        public AgentObject(IClass objclass) 
            : base(objclass)
        {
        }

        public IChannel Channel { get { return this.channel; } }

        public void Launch()
        {
            Thread thread = new Thread(new ParameterizedThreadStart(this.Execute));
            thread.IsBackground = true;
            thread.Start(Machine.Current);
        }

        public void SendInvoke(ICallable function, IBindingEnvironment environment, object[] arguments)
        {
            AgentTask task = new AgentTask() { Callable = function, Environment = environment, Arguments = arguments };
            this.channel.Send(task);
        }

        private void Execute(object parameter)
        {
            Machine machine = (Machine) parameter;

            machine.SetCurrent();

            while (true)
            {
                try
                {
                    object obj = this.channel.Receive();
                    AgentTask task = (AgentTask)obj;

                    task.Callable.Invoke(task.Environment, task.Arguments);
                }
                catch (Exception ex)
                {
                    // TODO review output
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
        }
    }

    internal class AgentTask
    {
        internal ICallable Callable;
        internal IBindingEnvironment Environment;
        internal object[] Arguments;
    }
}

