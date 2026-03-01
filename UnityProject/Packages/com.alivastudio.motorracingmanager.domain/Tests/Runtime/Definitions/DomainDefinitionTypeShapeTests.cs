using System;
using System.Linq;
using System.Reflection;
using MotorracingManager.Domain.Definitions;
using NUnit.Framework;

namespace MotorracingManager.Domain.Tests.Definitions
{
    public sealed class DomainDefinitionTypeShapeTests
    {
        [Test]
        public void DefinitionTypes_DoNotExposeFloatOrDoubleMembers()
        {
            var definitionTypes = new[]
            {
                typeof(SectorDef),
                typeof(SeriesDef),
                typeof(RulesetDef),
                typeof(TrackDef),
                typeof(PartTypeDef),
                typeof(SponsorObjectiveTemplate),
                typeof(SponsorDef),
                typeof(BuildingEffectDef),
                typeof(BuildingLevelDef),
                typeof(BuildingDef),
                typeof(ComponentDef),
                typeof(DriverArchetypeDef),
                typeof(DefinitionPack),
                typeof(ValidationResult),
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
