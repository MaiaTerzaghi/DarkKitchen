using System.Reflection;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Importers.Contracts;

namespace DarkKitchen.Importers.Loader;

public sealed class ReflectionImporterProvider(string pluginsPath) : IImporterProvider
{
    private readonly string _pluginsPath = pluginsPath;

    public IReadOnlyCollection<string> GetImporterNames()
    {
        return LoadAllImporters().Select(importer => importer.Name).ToList();
    }

    public IProductImporter GetByName(string name)
    {
        return LoadAllImporters().FirstOrDefault(importer => importer.Name == name)
            ?? throw new NotFoundException($"Importador '{name}' no encontrado.");
    }

    private IEnumerable<IProductImporter> LoadAllImporters()
    {
        if(!Directory.Exists(_pluginsPath))
        {
            return [];
        }

        return Directory.GetFiles(_pluginsPath, "*.dll")
            .SelectMany(LoadImportersFromAssembly);
    }

    private static IEnumerable<IProductImporter> LoadImportersFromAssembly(string dllPath)
    {
        var assembly = Assembly.LoadFrom(dllPath);
        return assembly.GetTypes()
            .Where(IsValidImporterType)
            .Select(type => (IProductImporter)Activator.CreateInstance(type)!);
    }

    private static bool IsValidImporterType(Type type)
    {
        return typeof(IProductImporter).IsAssignableFrom(type)
            && !type.IsInterface
            && !type.IsAbstract
            && type.GetConstructor(Type.EmptyTypes) != null;
    }
}
