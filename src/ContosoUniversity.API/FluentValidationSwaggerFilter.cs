﻿using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContosoUniversity.Web
{
    /// <summary>
    /// See <see href="https://stackoverflow.com/questions/44638195/fluent-validation-with-swagger-in-asp-net-core">this StackOverflow answer</see>.
    /// </summary>
    public class FluentValidationSwaggerFilter : ISchemaFilter
    {
        private readonly IValidatorFactory _factory;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="factory"></param>
        public FluentValidationSwaggerFilter(IValidatorFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        public void Apply(Schema model, SchemaFilterContext context)
        {
            // use IoC or FluentValidatorFactory to get AbstractValidator<T> instance
            var validator = _factory.GetValidator(context.SystemType);
            if (validator == null) return;
            if (model.Required == null)
                model.Required = new List<string>();

            var validatorDescriptor = validator.CreateDescriptor();
            foreach (var key in model.Properties.Keys)
            {
                foreach (var propertyValidator in validatorDescriptor
                    .GetValidatorsForMember(ToPascalCase(key)))
                {
                    if (propertyValidator is NotNullValidator
                      || propertyValidator is NotEmptyValidator)
                        model.Required.Add(key);

                    if (propertyValidator is LengthValidator lengthValidator)
                    {
                        if (lengthValidator.Max > 0)
                            model.Properties[key].MaxLength = lengthValidator.Max;

                        model.Properties[key].MinLength = lengthValidator.Min;
                    }

                    if (propertyValidator is RegularExpressionValidator expressionValidator)
                        model.Properties[key].Pattern = expressionValidator.Expression;

                    // Add more validation properties here;
                }
            }
        }

        /// <summary>
        /// To convert case as swagger may be using camel case
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static string ToPascalCase(string inputString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (inputString == null) return null;
            if (inputString.Length < 2) return inputString.ToUpper();
            return inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
        }
    }
}
