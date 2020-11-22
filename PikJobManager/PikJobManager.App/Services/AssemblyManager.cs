using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PikJobManager.Core;
using Module = Autofac.Module;

namespace PikJobManager.App.Services
{
    public static class AssemblyManager
    {
        public static List<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var modulesApp = $"{System.AppDomain.CurrentDomain.BaseDirectory}/Modules";
            foreach (string assemblyPath in Directory.GetFiles(modulesApp, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

                var assemblyName = assembly.GetName();
                if (assemblies.Any(x => x.GetName().Name == assemblyName.Name &&
                                        x.GetName().Version == assemblyName.Version))
                {
                    continue;
                }
                
                assemblies.Add(assembly);
            }

            return assemblies;
        }
        
        public static void GetTypes(List<Assembly> assemblies, out List<Type> typesToRegister, out List<Type> modules)
        {
            typesToRegister = new List<Type>();
            modules = new List<Type>();
            
            foreach (var assembly in assemblies)
            {
                var items = assembly.GetTypes()
                    .Where(x => x.IsClass && x.IsPublic && x.GetInterfaces().Contains(typeof(IPikJobManagerModule))).ToList();

                typesToRegister.AddRange(items);

                var moduleItems = assembly.GetTypes()
                    .Where(x => x.IsClass && x.IsPublic && x.BaseType == typeof(Module)).ToList();
                
                modules.AddRange(moduleItems);
            }
        }
    }
}