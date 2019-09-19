using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleIOC_Implementation
{
    static class IoC
    {
        static readonly IDictionary<Type, Type> types = new Dictionary<Type, Type>();

        public static void Register<TContract, TImplementation>()
        {
            types[typeof(TContract)] = typeof(TImplementation);
        }

        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static object Resolve(Type contract)
        {
            Type implementation = types[contract];
            ConstructorInfo constructor = implementation.GetConstructors()[0];
            ParameterInfo[] constructorParameters = constructor.GetParameters();
            if (constructorParameters.Length == 0)
            {
                return Activator.CreateInstance(implementation);
            }

            List<object> parameters = new List<object>(constructorParameters.Length);
            foreach (ParameterInfo parameterInfo in constructorParameters)
            {
                parameters.Add(Resolve(parameterInfo.ParameterType));
            }

            return constructor.Invoke(parameters.ToArray());
        }

        public static void Test()
        {
            IoC.Register<IFileSystemAdapter, FileSystemAdapter>();
            IoC.Register<IBuildDirectoryStructureService, BuildDirectoryStructureService>();
            IBuildDirectoryStructureService service = IoC.Resolve<IBuildDirectoryStructureService>();
        }
    }




    public interface IFileSystemAdapter { }

    public class FileSystemAdapter : IFileSystemAdapter { }

    public interface IBuildDirectoryStructureService { }

    public class BuildDirectoryStructureService : IBuildDirectoryStructureService
    {
        IFileSystemAdapter fileSystemAdapter;

        public BuildDirectoryStructureService(IFileSystemAdapter fileSystemAdapter)
        {
            this.fileSystemAdapter = fileSystemAdapter;
        }
    }
}
