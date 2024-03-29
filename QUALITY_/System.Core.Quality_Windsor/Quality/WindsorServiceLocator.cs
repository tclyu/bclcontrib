#region Foreign-License
//
// Author: Javier Lozano <javier@lozanotek.com>
// Copyright (c) 2009-2010, lozanotek, inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Modified: Sky Morey <moreys@digitalev.com>
//
#endregion
using System.Linq;
using System.Collections.Generic;
using Castle.Windsor;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
namespace System.Quality
{
    /// <summary>
    /// IWindsorServiceLocator
    /// </summary>
    public interface IWindsorServiceLocator : IServiceLocator
    {
        IWindsorContainer Container { get; }
    }

    /// <summary>
    /// WindsorServiceLocator
    /// </summary>
    [Serializable]
    public class WindsorServiceLocator : IWindsorServiceLocator, IDisposable
    {
        private IWindsorContainer _container;
        private WindsorServiceRegistrar _registrar;

        public WindsorServiceLocator()
            : this(CreateContainer()) { }

        public WindsorServiceLocator(IWindsorContainer container)
        {
            Container = container;
        }

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
                _registrar = null;
            }
        }

        public IServiceRegistrar GetRegistrar()
        {
            return _registrar;
        }

        public TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar
        {
            return (_registrar as TServiceRegistrar);
        }

        public TService Inject<TService>(TService instance)
            where TService : class
        {
            if (instance == null)
                return default(TService);
            instance.GetType()
                .GetProperties()
                .Where(property => property.CanWrite && Container.Kernel.HasComponent(property.PropertyType))
                .ToList()
                .ForEach(property => property.SetValue(instance, Container.Resolve(property.PropertyType), null));
            return instance;
        }

        public void Release(object instance)
        {
            Container.Release(instance);
        }

        public void Reset()
        {
            Dispose();
        }

        public TService Resolve<TService>()
            where TService : class
        {
            try
            {
                return Container.Resolve<TService>();
            }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }

        public TService Resolve<TService>(string key)
            where TService : class
        {
            try
            {
                return Container.Resolve<TService>(key);
            }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }

        public object Resolve(Type type)
        {
            try
            {
                return Container.Resolve(type);
            }
            catch (Exception ex) { throw new ServiceResolutionException(type, ex); }
        }

        public IList<TService> ResolveAll<TService>()
            where TService : class
        {
            return new List<TService>(Container.Kernel.ResolveAll<TService>());
        }

        public void TearDown<TService>(TService instance)
            where TService : class
        {
            if (instance != null)
                instance.GetType()
                    .GetProperties()
                    .Where(property => Container.Kernel.HasComponent(property.PropertyType))
                    .ToList()
                    .ForEach(property => Container.Release(property.GetValue(instance, null)));
        }

        public IWindsorContainer Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new WindsorServiceRegistrar(this, value);
            }
        }

        #region Domain specific
        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            var kernel = container.Kernel;
            kernel.Resolver.AddSubResolver(new ArrayResolver(kernel));
            kernel.Resolver.AddSubResolver(new ListResolver(kernel));
            return container;
        }
        #endregion
    }
}
