using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWL.Services.Core.Logging
{
	/// <summary>
	/// https://gist.github.com/pawelpabich/3066496
	/// </summary>
	/// <typeparam name="TLogger"></typeparam>
	public abstract class LogModule : Module
	{
		protected abstract ILogger CreateLoggerFor(Type type);

		protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
		{
			var type = registration.Activator.LimitType;
			if (HasPropertyDependencyOnLogger(type))
			{
				registration.Activated += InjectLoggerViaProperty;
			}

			if (HasConstructorDependencyOnLogger(type))
			{
				registration.Preparing += InjectLoggerViaConstructor;
			}
		}

		private bool HasPropertyDependencyOnLogger(Type type)
		{
			return type.GetProperties().Any(property => property.CanWrite && property.PropertyType == typeof(ILogger));
		}

		private bool HasConstructorDependencyOnLogger(Type type)
		{
			return type.GetConstructors()
					   .SelectMany(constructor => constructor.GetParameters()
															 .Where(parameter => parameter.ParameterType == typeof(ILogger)))
					   .Any();
		}

		private void InjectLoggerViaProperty(object sender, ActivatedEventArgs<object> @event)
		{
			var type = @event.Instance.GetType();
			var propertyInfo = type.GetProperties().First(x => x.CanWrite && x.PropertyType == typeof(ILogger));
			propertyInfo.SetValue(@event.Instance, CreateLoggerFor(type), null);
		}

		private void InjectLoggerViaConstructor(object sender, PreparingEventArgs @event)
		{
			var type = @event.Component.Activator.LimitType;
			@event.Parameters = @event.Parameters.Union(new[]
            {
                new ResolvedParameter((parameter, context) => parameter.ParameterType == typeof(ILogger), (p, i) => CreateLoggerFor(type))
            });
		}
	}
}
