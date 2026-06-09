using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: MetadataDump <assembly-path> <regex>");
    Environment.Exit(2);
}

var assemblyPath = args[0];
var pattern = new Regex(args[1], RegexOptions.IgnoreCase | RegexOptions.Compiled);

using var stream = File.OpenRead(assemblyPath);
using var peReader = new PEReader(stream);
var reader = peReader.GetMetadataReader();
var provider = new SignatureProvider(reader);

foreach (var typeHandle in reader.TypeDefinitions)
{
    var type = reader.GetTypeDefinition(typeHandle);
    var ns = reader.GetString(type.Namespace);
    var name = reader.GetString(type.Name);
    var fullName = string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";

    var typeMatches = pattern.IsMatch(fullName);
    var printedType = false;

    foreach (var methodHandle in type.GetMethods())
    {
        var method = reader.GetMethodDefinition(methodHandle);
        var methodName = reader.GetString(method.Name);
        if (!typeMatches && !pattern.IsMatch(methodName))
        {
            continue;
        }

        if (!printedType)
        {
            Console.WriteLine(fullName);
            printedType = true;
        }

        var signature = method.DecodeSignature(provider, null);
        Console.WriteLine($"  M {methodName}({string.Join(", ", signature.ParameterTypes)}) -> {signature.ReturnType}");
    }

    foreach (var fieldHandle in type.GetFields())
    {
        var field = reader.GetFieldDefinition(fieldHandle);
        var fieldName = reader.GetString(field.Name);
        if (!typeMatches && !pattern.IsMatch(fieldName))
        {
            continue;
        }

        if (!printedType)
        {
            Console.WriteLine(fullName);
            printedType = true;
        }

        Console.WriteLine($"  F {fieldName}");
    }

    foreach (var propertyHandle in type.GetProperties())
    {
        var property = reader.GetPropertyDefinition(propertyHandle);
        var propertyName = reader.GetString(property.Name);
        if (!typeMatches && !pattern.IsMatch(propertyName))
        {
            continue;
        }

        if (!printedType)
        {
            Console.WriteLine(fullName);
            printedType = true;
        }

        Console.WriteLine($"  P {propertyName}");
    }
}

internal sealed class SignatureProvider(MetadataReader reader) : ISignatureTypeProvider<string, object?>
{
    public string GetArrayType(string elementType, ArrayShape shape) => $"{elementType}[]";
    public string GetByReferenceType(string elementType) => $"{elementType}&";
    public string GetFunctionPointerType(MethodSignature<string> signature) => "fnptr";
    public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) => $"{genericType}<{string.Join(", ", typeArguments)}>";
    public string GetGenericMethodParameter(object? genericContext, int index) => $"!!{index}";
    public string GetGenericTypeParameter(object? genericContext, int index) => $"!{index}";
    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
    public string GetPinnedType(string elementType) => elementType;
    public string GetPointerType(string elementType) => $"{elementType}*";
    public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();
    public string GetSZArrayType(string elementType) => $"{elementType}[]";
    public string GetTypeFromDefinition(MetadataReader metadataReader, TypeDefinitionHandle handle, byte rawTypeKind)
    {
        var type = metadataReader.GetTypeDefinition(handle);
        var ns = metadataReader.GetString(type.Namespace);
        var name = metadataReader.GetString(type.Name);
        return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
    }
    public string GetTypeFromReference(MetadataReader metadataReader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var type = metadataReader.GetTypeReference(handle);
        var ns = metadataReader.GetString(type.Namespace);
        var name = metadataReader.GetString(type.Name);
        return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
    }
    public string GetTypeFromSpecification(MetadataReader metadataReader, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
    {
        var spec = metadataReader.GetTypeSpecification(handle);
        return spec.DecodeSignature(this, genericContext);
    }
}
