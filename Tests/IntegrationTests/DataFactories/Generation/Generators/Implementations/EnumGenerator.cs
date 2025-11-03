using AutoFixture.Kernel;

using Bogus;

using IntegrationTests.DataFactories.Generation.Generators.Interfaces;

namespace IntegrationTests.DataFactories.Generation.Generators.Implementations;

internal sealed class EnumGenerator(Faker faker) : ITypeDataGenerator
{
    public bool TryGenerate(Type type, string? name, ISpecimenContext context, out object? value)
    {
        if (type.IsEnum)
        {
            value = GenerateEnum(type);
            return true;
        }

        value = null;

        return false;
    }

    private object GenerateEnum(Type enumType)
    {
        var values = Enum.GetValues(enumType).Cast<object>().ToArray();
        var candidates = values.Length > 1 && Convert.ToInt32(values[0]) == 0 ? [.. values.Skip(1)] : values;
        return faker.PickRandom(candidates);
    }
}
