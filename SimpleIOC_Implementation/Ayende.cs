using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimpleIOC
{


    public class DemoContainer
    {
        public delegate object Creator(DemoContainer container);

        private readonly Dictionary<string, object> configuration
                       = new Dictionary<string, object>();
        private readonly Dictionary<Type, Creator> typeToCreator
                       = new Dictionary<Type, Creator>();

        public Dictionary<string, object> Configuration
        {
            get { return configuration; }
        }

        public void Register<T>(Creator creator)
        {
            typeToCreator.Add(typeof(T), creator);
        }

        public T Create<T>()
        {
            return (T)typeToCreator[typeof(T)](this);
        }

        public T GetConfiguration<T>(string name)
        {
            return (T)configuration[name];
        }


        static void Test()
        {
            DemoContainer container = new DemoContainer();
            //registering dependecies
            container.Register<IRepository>(delegate
            {
                return new NHibernateRepository();
            });
            container.Configuration["email.sender.port"] = 1234;
            container.Register<IEmailSender>(delegate
            {
                return new SmtpEmailSender(container.GetConfiguration<int>("email.sender.port"));
            });
            container.Register<LoginController>(delegate
            {
                return new LoginController(
                    container.Create<IRepository>(),
                    container.Create<IEmailSender>());
            });

            //using the container
            Console.WriteLine(
                container.Create<LoginController>().EmailSender.Port
                );
        }
    }

    class NHibernateRepository { }
    class LoginController { public SmtpEmailSender EmailSender; public LoginController(IRepository a, IEmailSender b) { } }
    class SmtpEmailSender { public int Port; public SmtpEmailSender(int i) { } }

    interface IEmailSender { }
    interface IRepository { }

}

