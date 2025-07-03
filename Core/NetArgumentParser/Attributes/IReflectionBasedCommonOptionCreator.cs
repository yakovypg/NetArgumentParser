using System.Reflection;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes;

public interface IReflectionBasedCommonOptionCreator
{
    bool CanCreateOption(object source, PropertyInfo propertyInfo);
    ICommonOption CreateOption(object source, PropertyInfo propertyInfo);
}
