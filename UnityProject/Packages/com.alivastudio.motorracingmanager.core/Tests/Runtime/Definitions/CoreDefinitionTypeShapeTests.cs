using System;
using System.Linq;
using System.Reflection;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;
using NUnit.Framework;

namespace MotorracingManager.Core.Tests.Definitions
{
    public sealed class CoreDefinitionTypeShapeTests
    {
        [Test]
        public void DefinitionTypes_DoNotExposeFloatOrDoubleMembers()
        {
            var definitionTypes = new[]
            {
                typeof(TyreCompoundDef),
                typeof(WeatherDef),
                typeof(SetupPenaltyEntry),
                typeof(EngineModeScalar),
                typeof(PartRankTimeCostTable),
                typeof(DriverPaceScalars),
                typeof(SetupScalars),
                typeof(KnowledgeScalars),
                typeof(TyreTemperatureScalars),
                typeof(FuelScalars),
                typeof(DraftingScalars),
                typeof(VarianceScalars),
                typeof(ScalarTables),
            };

            foreach (var type in definitionTypes)
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Assert.That(ContainsFloatOrDouble(property.PropertyType), Is.False, type.Name + "." + property.Name);
                }

                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    Assert.That(ContainsFloatOrDouble(field.FieldType), Is.False, type.Name + "." + field.Name);
                }
            }
        }

        private static bool ContainsFloatOrDouble(Type type)
        {
            if (type == typeof(float) || type == typeof(double))
            {
                return true;
            }

            if (type.IsArray)
            {
                return ContainsFloatOrDouble(type.GetElementType());
            }

            return type.IsGenericType && type.GetGenericArguments().Any(ContainsFloatOrDouble);
        }
    }
}
