@echo off
:: Release
xcopy Abstractions\System.Abstractions\bin\Release\System.Abstractions.* bin\ /Q /R /H /Y
::xcopy Abstractions\System.Web.Abstractions\bin\Release\System.Web.Abstractions.* bin\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality\bin\Release\System.Core.Quality.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Ninject\bin\Release\System.Core.Quality_Ninject.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_NServiceBus\bin\Release\System.Core.Quality_NServiceBus.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_NServiceBus_Community\bin\Release\System.Core.Quality_NServiceBus_Community.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_StructureMap\bin\Release\System.Core.Quality_StructureMap.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Unity\bin\Release\System.Core.Quality_Unity.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Windsor\bin\Release\System.Core.Quality_Windsor.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Mongo\bin\Release\System.Core.Quality_Mongo.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Sql\bin\Release\System.Core.Quality_Sql.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Serializer\bin\Release\System.Core.Quality_Serializer.* bin\Quality\ /Q /R /H /Y
xcopy Core\Quality\Library_\* bin\Quality\Library_\ /S /Q /R /H /Y
xcopy Core\SqlServer\System.Core.SqlServer\bin\Release\System.Core.SqlServer.* bin\SqlServer\ /Q /R /H /Y
xcopy Core\SqlServer\System.Core.SqlServer_Proxy\bin\Release\System.Core.SqlServer_Proxy.* bin\SqlServer\ /Q /R /H /Y
xcopy Core\System.ComponentModelEx\bin\Release\System.ComponentModelEx.* bin\ /Q /R /H /Y
xcopy Core\System.CoreEx\bin\Release\System.CoreEx.* bin\ /Q /R /H /Y
xcopy Core\System.DataEx\bin\Release\System.DataEx.* bin\ /Q /R /H /Y
xcopy Core\System.DirectoryServicesEx\bin\Release\System.DirectoryServicesEx.* bin\ /Q /R /H /Y
xcopy Extents\Digital.Cms\bin\Release\Digital.Cms.* bin\ /Q /R /H /Y
xcopy Services\System.ServiceModelEx\bin\Release\System.ServiceModelEx.* bin\ /Q /R /H /Y
xcopy Web\System.Web.Hosting\bin\Release\System.Web.Hosting.* bin\ /Q /R /H /Y
xcopy Web\System.Web.MvcEx\bin\Release\System.Web.MvcEx.* bin\ /Q /R /H /Y
xcopy Web\System.WebEx\bin\Release\System.WebEx.* bin\ /Q /R /H /Y
xcopy INTEGRATE_\WEB\System.WebEx.Integrate\bin\Release\System.WebEx.Integrate.* bin\INTEGRATE_\ /Q /R /H /Y
xcopy INTEROP_\CORE\System.Interop.Core\bin\Release\System.Interop.Core.* bin\INTEROP_\ /Q /R /H /Y

:: Debug
xcopy Abstractions\System.Abstractions\bin\Debug\System.Abstractions.* bin\Debug\ /Q /R /H /Y
::xcopy Abstractions\System.Web.Abstractions\bin\Debug\System.Web.Abstractions.* bin\Debug\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality\bin\Debug\System.Core.Quality.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Ninject\bin\Debug\System.Core.Quality_Ninject.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_NServiceBus\bin\Debug\System.Core.Quality_NServiceBus.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_NServiceBus_Community\bin\Debug\System.Core.Quality_NServiceBus_Community.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_StructureMap\bin\Debug\System.Core.Quality_StructureMap.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Unity\bin\Debug\System.Core.Quality_Unity.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Windsor\bin\Debug\System.Core.Quality_Windsor.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Mongo\bin\Debug\System.Core.Quality_Mongo.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Sql\bin\Debug\System.Core.Quality_Sql.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\System.Core.Quality_Serializer\bin\Debug\System.Core.Quality_Serializer.* bin\Debug\Quality\ /Q /R /H /Y
xcopy Core\Quality\Library_\* bin\Debug\Quality\Library_\ /S /Q /R /H /Y
xcopy Core\SqlServer\System.Core.SqlServer\bin\Debug\System.Core.SqlServer.* bin\Debug\SqlServer\ /Q /R /H /Y
xcopy Core\SqlServer\System.Core.SqlServer_Proxy\bin\Debug\System.Core.SqlServer_Proxy.* bin\Debug\SqlServer\ /Q /R /H /Y
xcopy Core\System.ComponentModelEx\bin\Debug\System.ComponentModelEx.* bin\Debug\ /Q /R /H /Y
xcopy Core\System.CoreEx\bin\Debug\System.CoreEx.* bin\Debug\ /Q /R /H /Y
xcopy Core\System.DataEx\bin\Debug\System.DataEx.* bin\Debug\ /Q /R /H /Y
xcopy Core\System.DirectoryServicesEx\bin\Debug\System.DirectoryServicesEx.* bin\Debug\ /Q /R /H /Y
xcopy Extents\Digital.Cms\bin\Debug\Digital.Cms.* bin\Debug\ /Q /R /H /Y
xcopy Services\System.ServiceModelEx\bin\Debug\System.ServiceModelEx.* bin\Debug\ /Q /R /H /Y
xcopy Web\System.Web.Hosting\bin\Debug\System.Web.Hosting.* bin\Debug\ /Q /R /H /Y
xcopy Web\System.Web.MvcEx\bin\Debug\System.Web.MvcEx.* bin\Debug\ /Q /R /H /Y
xcopy Web\System.WebEx\bin\Debug\System.WebEx.* bin\Debug\ /Q /R /H /Y
xcopy INTEGRATE_\WEB\System.WebEx.Integrate\bin\Debug\System.WebEx.Integrate.* bin\Debug\INTEGRATE_\ /Q /R /H /Y
xcopy INTEROP_\CORE\System.Interop.Core\bin\Debug\System.Interop.Core.* bin\Debug\INTEROP_\ /Q /R /H /Y
